<!--
 Copyright 2022 Deveel
 
 Licensed under the Apache License, Version 2.0 (the "License");
 you may not use this file except in compliance with the License.
 You may obtain a copy of the License at
 
     http://www.apache.org/licenses/LICENSE-2.0
 
 Unless required by applicable law or agreed to in writing, software
 distributed under the License is distributed on an "AS IS" BASIS,
 WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
 See the License for the specific language governing permissions and
 limitations under the License.
-->

# Sending Webhooks

The simple act of sending Webhooks to receivers is not depdenent from the existence of subscriptions, and can be executed throug direct invocations to instances of the `IWebhookSender` service.

## Configuring the Webook Sender

Although a Webhook Sender can be instantiated, configured used directly, the framework provides a set of extension methods that can be used to configure the service in a more convenient way, especially in contexts that require the use of Dependency Injection.

Assuming you are working on a traditional _[ASP.NET application model](https://docs.microsoft.com/en-us/aspnet/core/fundamentals/?view=aspnetcore-6.0&tabs=windows)_, you can start using the sender functions of this framework through the _Dependency Injection_ (DI) pattern, including a default implementation of the sender during the Startup of your application, by calling the `.AddWebhooks()` extension method provided by _Deveel Webhooks_, and optionally configuring the behavior of the delivery:

``` csharp
using System;

using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

using Deveel.Webhooks;

namespace Example {
    public class Startup {
        public Startup(IConfiguration config) {
            Configuration = config;
        }

        public IConfiguration Configuration { get; }

        public void Configure(IServiceCollection services) {
            // ... add any other service you need ...

            // this call adds the basic services for sending of webhooks
            services.AddWebhookSender<MyWebook>(webhooks => {
                // Optional: if not configured by you, the service
                // will use the default configurations..
                webhooks.Configure(options => {
                    // ... configure the options ...
                });
            });
        }
    }
}

```

### Webhook Signatures

A recommended practice for Webhooks is to sign the payloads of the messages, so that the receiver can verify the authenticity of the message. The framework provides a mechanism to sign the payloads of the messages, and to verify the signatures of the incoming messages.

The framework provides by default an implementation of a signature provider (the `IWebhookSigner` contract), that computes signatures using the 'HMAC-SHA-256' algorithm, but it is possible to add custom ones through the call to `.AddSigner<TSigner>()` of the service builder.

``` csharp
using System;

using Microsoft.Extensions.Configuration;

using Deveel.Webhooks;

namespace Example {
	public class Startup {
		public Startup(IConfiguration config) {
			Configuration = config;
		}
		public IConfiguration Configuration { get; }
		public void Configure(IServiceCollection services) {
			// ... add any other service you need ...
			// this call adds the basic services for sending of webhooks
			services.AddWebhookSender<MyWebook>(webhooks => {
				// Optional: if not configured by you, the service
				// Add a custom signature provider
				webhooks.AddSigner<MySigner>();
			});
		}
	}
}
```

### Webhook Serialization

The most common format for the payloads of Webhooks is the JSON format, but it is possible to use also the XML format, as long as the receiver is able to understand it. The framework provides a mechanism to serialize the payloads of the messages sent to the receivers.

When initializing the sender, a default serializer for each format is added to the service, but it is possible to add custom ones through the call to `.UseJsonSerializer<TSerializer>()` or `.UseXmlSerializer<TSerializer>()` of the service builder.

``` csharp

using System;

using Microsoft.Extensions.Configuration;

using Deveel.Webhooks;

namespace Example {
	public class Startup {
		public Startup(IConfiguration config) {
			Configuration = config;
		}

		public IConfiguration Configuration { get; }

		public void Configure(IServiceCollection services) {
			// ... add any other service you need ...
			// this call adds the basic services for sending of webhooks
			services.AddWebhookSender<MyWebook>(webhooks => {
				webhooks.UseJsonSerializer<MyJsonSerializer>();
				webhooks.UseXmlSerializer<MyXmlSerializer>();
			});
		}
	}
}
```

## Using the Webhook Sender

At this point you can obtain from the service provider an instance of the `IWebhookSender<MyWebhook>` that is configured and ready to be used.

Assuming your application is using the _ASP.NET Core_ framework, you can inject the service in your controllers, and use it to send Webhooks to the receivers:

``` csharp
using System;

using Deveel.Webhooks;

using Example.WebModels;

namespace Example {
    [ApiController]
    [Route("webhook")]
    public class WebhookController : ControllerBase {
        private readonly IWebhookSender<MyWebhook> webhookSender;

        public WebhookController(IWebhookSender<MyWebhook> webhookSender) {
            this.webhookSender = webhookSender;
        }

        [HttpPost]
        public async Task<IActionResult> Post([FromBody] WebhookModel webhook) {
            // First you must transform the model to a MyWebhook instance
            // that is compatible with the sender.. we assume that the
            // you are using the Adapt() extension method provided by
            // Mapperly ...

            var myWebhook = webhook.Body.Adapt<MyWebhook>();
            var destination = webhook.Destination.Adapt<WebhookDestination>();

            // The service sends the webhook to the destination address
            // according to the configurations provided
            var result = await webhookSender.SendAsync(destination, myWebhook, HttpContext.RequestAborted);

            // The result of the send is not serializable and requires
            // a transformation to a compatible object
            var resultModel = result.Adapt<WebhookResultModel>();

            return Ok(resultModel);
        }
    }
}

```

### Webhook Destinations

While the webhook represents a message that is sent to a receiver, the destination is the address of the receiver, complimented with additional configurations that can override the defaults set during the building of the sender service. 

In the context of _simple sending_ of Webhooks, the destination is represented by an instance of the `WebhookDestination` class.

``` csharp

var destination = new WebhookDestination("https://my-webhook-receiver.com/events/webhooks")
    .WithRetry(options => {
        options.RetryCount = 3;
		options.RetryDelay = TimeSpan.FromSeconds(5);
    });

var result = await webhookSender.SendAsync(destination, myWebhook);

```

### Webhook Delivery Results

The result of the sending of a Webhook is represented by an instance of the `WebhookDeliveryResult<MyWebhook>` class, that provides an aggregation of the results of the delivery attempts done by the sender.

During the process of sending webhooks, the sender can perform multiple attempts to deliver the message to the receiver (depending on the retry configuration), and the result of the delivery is represented by an instance of the `WebhookDeliveryAttempt` class: a result can be considered successful if at least one attempt was successful, and it can be considered failed if all the attempts failed.

You can use the `WebhookDeliveryResult` class to check the status of the delivery, and to retrieve the results of the attempts.