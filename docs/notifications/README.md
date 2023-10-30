# Webhook Notifications

The notification process of a webhook introduces elements of automation, putting together the [_subscription management_](webhook-subscriptions-management/) and the [_sending of webhooks_](../sending-webhooks/) processes, and an optional step of [_data transformation_](custom\_datafactory.md) (to resolve event information in a fully formed object to be transferred).

This process is dependent on some components:

* **Webhook Subscriptions** - To be able to notify a webhook, the notifier must be aware of which application has subscribed to the notification of the given event.
* **Webhook Factory** - The event that occurred must be converted into complete information to be notified.
* **Sending Webhooks** - The webhooks must be delivered to the destination, through serialization, signature, and retries
* **Logging Delivery Results** - In scenarios of usage as background service, the logging of the results of the delivery process provides observability of the performances of the notification

## Install the Required Library

The overall set of libraries are available through [NuGet](https://nuget.org), and can be installed and restored easily once configured in your projects.

### Requirements

To implement the core functionalities of webhook notifications, you must install the `Deveel.Webhooks` library.

The library currently supports both the `.NET 6.0` and `.NET 7.0` runtimes.

### Install the Package

You can install it through the `dotnet` command line, using the command

```sh
$ dotnet add package Deveel.Webhooks
```

Or by editing your `.csproj` file and adding a `<PackageReference>` entry.

```xml
<Project Sdk="Microsoft.NET.Sdk">
  <PropertyGroup>
    <TargetFramework>ne7.0</TargetFramework>
    ...
  </PropertyGroup>

  <ItemGroup>
    <PackageReference Include="Deveel.Webhooks" Version="2.1.1" />
    ...
  </ItemGroup>
</Project>
```

## Registering the Webhook Service

To begin using the functions of the webhook service, all that you need is to invoke the `.AddWebhookNotifier()` function of the service collection in the instrumentation of your application: this will add the required default services to the container, enabling the application to start notifying events to the registered recipients.

For example:

```csharp
var builder = services.AddWebhookNotifier<MyWebhook>();
```

The method above registers the default services required to notify webhooks, and returns an instance of `WebhookNotifierBuilder<TWebhook>` that can be used to configure the behavior of the service.

It also registers a default implementation of the `IWebhookNotifier<TWebhook>` service, that can be used to trigger the notification process: please refer to the [specific chapter](webhook_notifier.md) to learn more about this service.

### The Webhook Scope

The Webhook Notifier service is designed to be scoped to a specific type of webhook, isolating services like the subscription resolver, the webhook factory, or the sender service to the specific type of webhook.

This allows to have multiple instances of the service, each one dedicated to a specific type of webhook, and each one with its own configuration, running in the same application.

#### Example Registration

For example, assuming you are working on a traditional [_ASP.NET application model_](https://docs.microsoft.com/en-us/aspnet/core/fundamentals/?view=aspnetcore-6.0\&tabs=windows), you can enable these functions like this:

```csharp
using System;

using Deveel.Webhooks;

using Example.WebModels;

namespace Example {
    public class Startup {
        public Startup(IConfiguration config) {
            Configuration = config;
        }

        public IConfiguration Configuration { get; }

        public void Configure(IServiceCollection services) {
            // ... and use the MongoDB webhook subscription layer ...
            services.AddWebhookSubscriptions<MongoWebhookSubscription>(subs => 
                subs.UseMongo(Configuration.GetConnectionString("MongoWebhooks")));

            // this call adds the basic services for sending of webhooks
            services.AddWebhookNotifier<MyWebhook>(webhooks => {
                // by default a IWebhookSubscriptionResolver<TWebhook> service
                // is registered, that is a wrapper around the IWebhookSubscriptionRepository<MongoWebhookSubscription> available
                // from the previous call...

                // Optional: add a filter engine that handles string-based "linq" filters
                webhooks.AddDynamicLinqFilters();

                // Optional: configure the delivery behavior of the sender service
                webhooks.ConfigureDelivery(delivery => {
                    ...
                });
            });
        }
    }
}
```

### The `IWebhookNotifier<TWebhook>` Service

The _Deveel Webhook_ framework implements the notification functions through instances of the `IWebhookNotifier<TWebhook>` service, which is designed to trigger the notification process from an Event.

Registering the service also registers by default the `WebhookNotifier<TWebhook>` service, which implements the notification process by depending on external services for the Webhook Subscription resolution, the building of Webhooks, and the logging of Delivery Results.

During the configuration of the service, it is possible to replace the default implementation with a custom one, or to configure the default one with additional services, using the instance of the `WebhookNotifierBuilder<TWebhook>` returned by the `.AddWebhookNotifier()` method.

See the [specific chapter](webhook_notifier.md) to learn more about the default service.

### Using the Webhook Notifier (ASP.NET Core Example)

Once your application's service collection has been built, an instance of the `IWebhookNotifier<TWebhook>` service will be available through Dependency Injection.

Assuming you are running a ASP.NET Core service, using a traditional MVC model.

```csharp
namespace Example {
    [ApiController]
    [Route("webhook")]
    public class WebhookController : ControllerBase {
        private readonly IWebhookNotifier<MyWebhook> webhookNotifier;

        public WebhookController(IWebhookNotifier<MyWebhook> webhookNotifier) {
            this.webhookNotifier = webhookNotifier;
        }

        [HttpPost("{tenantId}")]
        public async Task<IActionResult> Post([FromRoute]string tenantId, [FromBody] EventModel webEvent) {
            // The service requires an instance of 'EventInfo'
            // to trigger the notification process and we assume
            // your EventModel class can create one...
            var eventInfo = webEvent.AsEventInfo();

            // The service then tries to notify the event to the
            // registered subscribers, and returns an aggregation of
            // all the results of the delivery process
            var result = await webhookNotifier.NotifyAsync(eventInfo, HttpContext.RequestAborted);

            // The object returned by the notifier is not serializable and
            // to return the object through ASP.NET you need an instance
            // supporting serialization
            var resultModel = WebhookNotificationResultModel.FromResult(result);

            return Ok(resultModel);
        }
    }
}
```

## Notifications for Multi-Tenancy Scenarios
