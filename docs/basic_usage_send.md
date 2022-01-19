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

As mentioned above, the simple act of sending webhooks to receivers is not depdenent from the existence of subscriptions, but the process might still be configured.

---

Still assuming you are working on a traditional _[ASP.NET application model](https://docs.microsoft.com/en-us/aspnet/core/fundamentals/?view=aspnetcore-5.0&tabs=windows)_, you can start using the sender functions of this framework through the _Dependency Injection_ (DI) pattern, including a default implementation of the sender during the Startup of your application, by calling the `.AddWebhooks()` extension method provided by _Deveel Webhooks_, and optionally configuring the behavior of the delivery:

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
            services.AddWebhooks(webhooks => {
                webhooks.ConfigureDelivery(delivery => {
                    // Instructs the sender to sign the webhooks
                    delivery.SignWebhooks = true;
                    // Specifies which algorithm to use for the signature
                    delivery.SignatureAlgorithm = WebhookSignatureAlgortihms.HmacSha256;
                    // ... and to place the signature in the header of the request
                    delivery.SignatureLocation = WebhookSignatureLocation.Header;
                    // ... naming the signature header
                    delivery.SignatureHeaderName = "X-WEBHOOK-SIG"

                    // We want to limit to just 1 attempt
                    delivery.MaxAttemptCount = 1;
                    // ... and timeout after 2 seconds if not delivered
                    delivery.TimeOut = TimeSpan.FromSeconds(2);

                    // We also want to include all the default fields (event ID, event name, time-stamp, etc.)
                    delivery.IncludeFields = WebhookFields.All;

                    // ... and some more cosmetics for the payload
                    delivery.JsonSerializerSettings.NullValueHandling = NullValueHandling.Ignore;
                    delivery.JsonSerializerSettings.Formatting = Formatting.Indent;
                });
            });
        }
    }
}

```

_Please note_: The framework provides by default an implementation of a signature provider (the `IWebhookSigner` contract), that computes signatures using the 'HMAC-SHA-256' algorithm, but it is possible to add custom ones through the call to `.AddSigner()` of the service builder.

---

At this point you can obtain from the service provider an instance of the `IWebhookSender` that is configured and ready to be used.

``` csharp
using System;

using Deveel.Webhooks;

using Example.WebModels;

namespace Example {
    [ApiController]
    [Route("webhook")]
    public class WebhookController : ControllerBase {
        private readonly IWebhookSender webhookSender;

        public WebhookController(IWebhookSender webhookSender) {
            this.webhookSender = webhookSender;
        }

        [HttpPost]
        public async Task<IActionResult> Post([FromBody] WebhookModel webhook) {
            // We assume that the implementation of the 'WebhookModel' class
            // inherits from the IWebhook contract of the framework

            // The service sends the webhook to the destination address
            // according to the configurations provided
            var result = await webhookSender.SendAsync(webhook);

            // The result of the send is not serializable and requires
            // a transformation to a compatible object
            var resultModel = WebhookResultModel.FromResult(result);

            return Ok(resultModel);
        }
    }
}

```
