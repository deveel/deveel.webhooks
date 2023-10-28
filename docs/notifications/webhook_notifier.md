# The WebhookNotifier Service

An default implementation of the `IWebhookNotifier<TWebhook>` service is provided by the framework, as the `WebhookNotifier` class, that executes the following steps:

1. The service firsts tries to resolve any webhook subscriptions matching the _event type_ and the _tenant identifier_ (_in case of multi-tenant scenarios_).
2. Tries to resolve any service implementing `IWebhookDataFactory`, and trasnforms the event data into a new form (eg. _resolving a database record from an identifier_)
3. All the matching subscriptions are iterated and a webhook object is constructed for each of them, using the _event metdata_ (eg. timestamp, stream name, type, etc.), the _subscription data_ and the _event data_ (or the result of a transformation, if any happened in the previous step)
4. The webhook object is matched against the subscription as follows: a. Not active subscriptions are skipped and will not be notified b. If the subscription includes any filter, these ones are matched against the webhook object formed in the previous steps, to determine if the conditions defined by the subscription are met
5. Webhooks are sent to the receiving end-point of the subscription

## Using the Webhook Notifier (ASP.NET Core)

Still assuming you are working on a traditional [_ASP.NET Core application model_](https://docs.microsoft.com/en-us/aspnet/core/fundamentals/?view=aspnetcore-6.0\&tabs=windows), you can start using the notification functions of this framework through the _Dependency Injection_ (DI) pattern, including a default implementation of the sender during the Startup of your application, by calling the `.AddWebhookNotifier<MyWebhook>()` extension method provided by _Deveel Webhooks_, mixing the configurations of managament and delivery.

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
            // ... add any other service you need ...

            // this call adds the basic services for sending of webhooks
            services.AddWebhookNotifier<MyWebhook>(webhooks => {
                // This registers a subscription resolver that is backed
                // by the a Mongo database (from the Deveel.Webhooks.Mongo package)
                webhooks.UseMongoSubscriptionResolver();

                // for this implementation we use a Mongo database as
                // persistent layer of the subscriptions
                wekhooks.UseMongoDb(mongo => {
                    mongo.UseConnectionString(Configuration
                         .GetConnectionString("MongoWebhooks"));
                });

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

The builder of the notifier service registers by default the `WebhookSender` service, that is responsible for the delivery of the webhooks to the receiving end-point of the subscription.

It is possible to configure the delivery behavior of the sender service by calling the `.ConfigureDelivery()` method of the builder, and this will control the behavior of the sender service, including the retry policy, the timeout and the number of retries.

By default the notifier service is not provided with any filter evaluation engine, and this means that the subscriptions will be matched against the webhook object without any filter evaluation, even if they include a filter.

To be able to evaluate the filters of subscriptions, you need to register an instance of `IWebhookFilterEvaluator` service.

### Consuming the Notifier Service

After this you will have an instance of `IWebhookNotiier<MyWebhook>` instance that can be used witin your code for triggering the process described above.

_**Note**: Since in this example no `IWebhookDataFactory` instance has been registered, the webhook payload will contain the same data transported by the event._

```csharp
namespace Example {
    [ApiController]
    [Route("webhook")]
    public class WebhookController : ControllerBase {
        private readonly IWebhookNotifier webhookNotifier;

        public WebhookController(IWebhookNotifier webhookNotifier) {
            this.webhookNotifier = webhookNotifier;
        }

        [HttpPost("{tenantId}")]
        public async Task<IActionResult> Post([FromRoute]string tenantId, [FromBody] EventModel webEvent) {
            // The service requires an instance of 'EventInfo'
            // to trigger the notification process and we assume
            // your EventModel class can create one
            var eventInfo = webEvent.AsEventInfo();

            // Since the resolution of the subscriptions to the event is
            // multi-tenant, we require a tenant identifier here:
            // this can be in the route of the method, part of the body
            // or configured at application level: your design :)
            var result = await webhookNotifier.NotifyAsync(tenantId, eventInfo, HttpContext.RequestAborted);

            // The object returned by the notifier is not serializable and
            // to return the object through ASP.NET you need an instance
            // supporting serialization
            var resultModel = WebhookNotificationResultModel.FromResult(result);

            return Ok(resultModel);
        }
    }
}

```

## Webhook Subscription Resolvers

In order to resolve the subscriptions to a given event, the `IWebhookSubscriptionResolver` service is used: this service is responsible for the retrieval of the subscriptions that match the event type, in scope of a tenant.

Some of the libraries implementing a persistent layer for the subscriptions, provide an implementation of the `IWebhookSubscriptionResolver` service, that can be used to resolve the subscriptions from the persistent layer.

Future implementations of the contract might provide a way to resolve the subscriptions from a cache, or from a remote service.

## Logging Results

The default behavior of the notifier service is to notify to a multitude of subscribers for each single event, and this can be a complex process, that can fail for a number of reasons: to be able to track the delivery of the webhooks, the framework provides the `IWebhookDeliveryResultLogger` contract, that can be used to store the results of the delivery of the webhooks, while the notifier service is executing.
