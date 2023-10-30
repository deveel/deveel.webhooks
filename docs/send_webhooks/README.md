# Sending Webhooks

The simple act of sending Webhooks to receivers is not dependent on the existence of subscriptions and can be executed through direct invocations to instances of the `IWebhookSender` service.

In fact, the Webhook Sender component of the framework provides the following capabilities:

* **Serialization of the Payload** - The content of the payload of the webhook is serialized according to the configurations and format (eg. _JSON_, _XML_, _Form-Encoded_, etc.)
* **Signature** - The sender computes a signature of the webhook payload, to provide the receiver a proof of its integrity
* **Delivery Retry** - The delivery of the webhooks is retried until successful, or until a breaking condition is met

This component doesn't provide any capabilities for managing subscriptions to event notifications, or to automate the notification of the events: see the Webhook Notification chapter for learning how to activate it.

## Install the Required Libraries

The overall set of libraries are available through [NuGet](https://nuget.org), and can be installed and restored easily once configured in your projects.

### Requirements

The library currently suppots both the `.NET 6.0` and `.NET 7.0` runtimes.

### Installing the Package

You can do this by using the .NET command line on the root folder of your project

```bash
dotnet add package Deveel.Webhooks.Sender
```

Alternatively, you can add a reference in your project file

```xml
<Project Sdk="Microsoft.NET.Sdk">
  <PropertyGroup>
    <TargetFramework>ne7.0</TargetFramework>
    ...
  </PropertyGroup>

  <ItemGroup>
    <PackageReference Include="Deveel.Webhooks.Sender" Version="2.1.1" />
    ...
  </ItemGroup>
</Project>
```

### Registering the Webook Sender

The most common way to use the Webhook Sender is to register it in the collection of services of your application, using the _dependency injection_ pattern to obtain it.

You can use one of the overloads of the extension method `.AddWebhookSender<TWebhook>()` to the `IServiceCollection` contract, which will return a builder object that can be used to configure the service.

For example:

```csharp
services.AddWebhookSender<MyWebhook>()
    .Configure(options => {
        // ...
    });
```

or even simplier:

```csharp
// To use the default configurations
services.AddWebhookSender<MyWebhook>();
```

This method will register the default implementation of the `IWebhookSender<TWebhook>`, returning a builder object for further configurations of the service.

See the [configuration chapters](configuring-the-sender.md) for further information about the available options to configure the sender.

### The Webhook Scope

The Webhook Sender service is scoped to the type of the webhook: this means that you can register multiple instances of the service, each one for a specific type of webhook.

You will find that this is useful when you need to send different types of webhooks, with different configurations, using the same infrastructure or application.

The same scoping mechanism is inherited by the `IWebhookNotifier<TWebhook>` service: see the documentation of the [Webhook Notification](../notifications/README.md) chapter for further information.

#### Example Registration

Assuming you are working on a traditional [_ASP.NET application model_](https://docs.microsoft.com/en-us/aspnet/core/fundamentals/?view=aspnetcore-6.0\&tabs=windows), you can start using the sender functions of this framework through the _Dependency Injection_ (DI) pattern, including a default implementation of the sender during the Startup of your application, by calling the `.AddWebhooks()` extension method provided by _Deveel Webhooks_, and optionally configuring the behavior of the delivery:

```csharp
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

## Using the Webhook Sender

At this point, you can obtain from the service provider an instance of the `IWebhookSender<MyWebhook>` that is configured and ready to be used.

If you have not overridden the default sender service (using the `.UserSender<TSender>()` method of the builder object), you will also have an instance of the `WebhookSender<MyWebhook>` client service available.

#### Example Usage

Assuming your application is using the _ASP.NET Core_ framework, you can inject the service in your controllers, and use it to send Webhooks to the receivers:

```csharp
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

While q webhook represents a message that is sent to a receiver, the Destination is the address of the receiver, complimented with additional configurations that can override the default configurations of the sender service.

In the context of _simple sending_ of Webhooks, the destination is represented by an instance of the `WebhookDestination` structure.

```csharp
var destination = new WebhookDestination("https://my-webhook-receiver.com/events/webhooks")
    .WithRetry(options => {
        options.RetryCount = 3;
        options.RetryDelay = TimeSpan.FromSeconds(5);
    });

var result = await webhookSender.SendAsync(destination, myWebhook);
```

### Webhook Delivery Results

The result of the sending of a Webhook is represented by an instance of the `WebhookDeliveryResult<TWebhook>` class, that provides an aggregation of the delivery attempts done by the sender.

The sender can perform multiple attempts to deliver the message to the receiver (depending on the retry configuration), and the result of the delivery is represented by an instance of the `WebhookDeliveryAttempt` class: a result can be considered successful if at least one attempt was successful, and it can be considered failed if all the attempts failed.

You can use the `WebhookDeliveryResult` class to check the status of the delivery, and to retrieve the results of the attempts.
