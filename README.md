[![Code Coverage](https://codecov.io/gh/deveel/deveel.webhooks/branch/main/graph/badge.svg?token=BKRX2N1IZ1)](https://codecov.io/gh/deveel/deveel.webhooks)

# Deveel Webhooks

This project provides a set of .NET tools for the management of subscriptions to events, basic transformations and notifications of such event occurrences (_webhooks_): in a global design scope, this model enables event-driven architectures, triggering system processes upon the occurrence of expected occurrences from other systems.

Although this integration model is widely adopted by major service providers (like _[SendGrid](https://docs.sendgrid.com/for-developers/tracking-events/getting-started-event-webhook)_, _[Twilio](https://www.twilio.com/docs/usage/webhooks)_, _[GitHub](https://docs.github.com/en/developers/webhooks-and-events/webhooks/about-webhooks)_, _[Slack](https://api.slack.com/messaging/webhooks)_, etc.), there is no formal protocol or authority that would enforce a compliance (like for other cases, such as OpenID, OpenAPI, etc.).

Anyway, a typical implementation consists of the following elements:

* Webhooks are transported through _HTTP POST_ callbacks
* The webhook payload is represented as a JSON object
* The webhook payload includes properties that describe the type of event and the time-stamp of the occurrence
* An optional signature in the header of the request or a query-string parameter ensures the authenticity of the caller

## Motivation

While working on a .NET Core/.NET 5 *aaS (_as-a-Service_) project that functionally required the capability of users of the service being able to create system-to-system subscriptions and notifications (typically named _webhooks_), I started my design with the ambition to use existing solutions, to avoid the bad practice of _reinventing the wheel_, but I ended up frustrated in such ambition:

* [Microsoft's ASP.NET Webhooks](https://github.com/aspnet/WebHooks) project was archived and moved back to the [Microsoft ASP Labs](https://github.com/aspnet/AspLabs), not providing any visibility on its status, and not being compatible with .NET Core (and even less with the stack .NET 5/6)
* Even in its _experimental_ status, the **Microsoft ASP.NET Webhooks** framework removed the capability of handling subscriptions, being mainly focused on _receivers_
* Alternative implementations were included and organic part of frameworks (like [ASP.NET Boilerplate](https://github.com/aspnetboilerplate/aspnetboilerplate)), that would have forced me to adopt the the entirety of such frameworks, beyond my design intentions

## NuGet Libraries

This framework is composed by a set of libraries enabling some functionalities

| Library                         |   Description                                                                       |
|---------------------------------|-------------------------------------------------------------------------------------|
| Deveel.Webhooks.Model           | Defines the contracts of the webhooks information and tranasformation  model        |
| Deveel.Webhooks                 | Provides abstractions and basic implementations of the webhook sending capabilities |
| Deveel.Webhooks.Service         | Implements the functions for the management of webhook subscriptions |
| Deveel.Webhooks.Service.MongoDb | An implementation of the storage layer of subscriptions and webhook logging that uses MongoDB |
| Deveel.Webhooks.DynamicLinq     | A filter engine that resolves through Dynamic LINQ expressions |

You can obtain the stable versions of these libraries from the [NuGet Official](https://nuget.org) channel.

For the _nighly builds_ and previews you can restore from the [Deveel Package Manager](https://github.com/orgs/deveel/packages). 

## Basic Usage - Outbound

The overall design of this little framework is open and extensible (implementing the traditional [Open-Closed Principle](https://en.wikipedia.org/wiki/Open%E2%80%93closed_principle)), that means base contracts can be extended, composed or replaced.

### Webhook Subscription Management

The process of sending _webhooks_ to recipient \[systems\] (implemented by the `IWebhookSender` contract), at least in the model provided by _Deveel Webhooks_, is not technically dependent from the existence of the subscription management component: that means you might implement your own subscription management (see the section on _Advanced Usage_ for further information on how to do it).

In fact this requirement exists for the _notification_ scenarios (implemented by the `IWebhookNotifier`), in which the framework resolves the matching subscriptions to an event, forms the webhook and delivers it through the sender.

Anyway the framework provides an implementation for the management and resolution of subscriptions to certain type of events, and eventually other conditions (_filters_) to ease the process of notification.

---

Assuming you are working on a traditional _[ASP.NET application model](https://docs.microsoft.com/en-us/aspnet/core/fundamentals/?view=aspnetcore-5.0&tabs=windows)_, you can start using the management functions of this framework through the _Dependency Injection_ (DI) pattern, including the base implementations during the Startup of your application, calling the `.AddWebhooks()` extension method provided by _Deveel Webhooks_:

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
                // with this we include the subscription management services
                webhooks.UseSubscriptionManager();

                // for this implementation we use a Mongo database as
                // persistent layer of the subscriptions
                wekhooks.UseMongoDb(mongo => {
                    mongo.SetConnectionString(Configuration.GetConnectionString("MongoWebhooks"))
                         .SetDatabase("example_app")
                         .SetSubscriptionsCollection("webhook_subs");
                });
            });
        }
    }
}

```

You can proceed by creating a controller `WebhookSubscriptionController` and including the contract `IWebhookSubscriptionManager` as argument of the constructor>

``` csharp
using System;

using Deveel.Webhooks;

using Example.WebModels;

namespace Example {
    [ApiController]
    [Route("webhook/subscription")]
    public class WebhookSubscriptionController : ControllerBase {
        private readonly IWebhookSubscriptionManager subscriptionManager;

        public WebhookSubscriptionController(IWebhookSubscriptionManager subscriptionManager) {
            this.subscriptionManager = subscriptionManager;
        }

        [HttpPost("{tenantId}")]
        public async Task<IActionResult> Post([FromRoute]string tenantId, [FromBody] WebhookSubscriptionModel subscription) {
            // we need a WebhookSubscriptionInfo object to create a webhook subscription,
            // and this implementation assumes that your web model WebhookSubscriptionModel
            // provides a method to create one instance of it
            var subscriptionInfo = subscription.AsSubscriptionInfo();

            // The system is natively multi-tenant, that means an identifier of the owning
            // tenant of the subscriptions is needed (in future versions it might be an optional capability)
            var result = await subscriptionManager.CreateAsync(tenantId, subscriptionInfo, HttpContext.RequestAborted);

            // The result of a successfull creation is a IWebhookSubscription instance, that
            // you need to convert to an ASP.NET serializable model...
            var resultModel = WebhookSubscriptionModel.FromSubscription(result);

            // In this implementation we respond with a 201 code, the location of the GET /webhook/subscription/{tenantId}/{subscriptionId}
            // and the subscription object just created
            return CreatedAtAction("Get", new { tenantId, id = result.SubscriptionId }, resultModel);
        }

        [HttpGet("{tenantId}/{id}")]
        public async Task<IActionResult> Get([FromRoute]string tenantId, [FromRoute] string id) {
            var subscription = await subscriptionManager.GetAsync(tenantId, id, HttpContext.RequestAborted);

            if (subscription == null)
                return NotFound();

            // The returned object is a IWebhookSubscription instance, that
            // you need to convert to an ASP.NET serializable model...
            var resultModel = WebhookSubscriptionModel.FromSubscription(subscription);

            return Ok(resultModel);
        }
    }
}

```

**Note**: The `IWebhookSubscriptionManager` contract provides more functions (for paginated lists, updates, deletions, state changing, etc.), not displayed in this simple example.

### Webhook Sending

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

### Webhook Notification

The notification process of a webhook introduces elements of automation, putting together the _subscription management_ and the _sending_ processes (described above), and an optional _transformation_ process, to resolve event information in a full formed object to be transferred (eg. _the reference to a contact is used to resolve the contact object, that is then included in the payload of the webhook_).

The _Deveel Webhook_ framework implements these functions through an instance of the `IWebhookNotifier` contract, that uses a `IWebhookSubscriptionResolver` (by default backed by the subscription manager) and the `IWebhookSender` instance configured.

The notification process is triggered by an event (represented by the `EventInfo` object):

1. The event is matched against any object transformers (`IWebhookDataFactory`) handling it
2. An instance of a webhook is constructed containing the event data, or the result of a transformation (if any occurred)
3. The webhook object is matched against the available subscriptions in the scope of the tenant
   a. Only active subscriptions are considered
   b. The event type is firstly matched
   c. If the subscription includes any other additional filter, these ones are matched against the webhook object formed in the previous steps (and using the filter engine)
4. Webhooks are send to all recerivers matching the subscription conditions
5. Optionally the result of the notification is stored for later reporting an review

---

Still assuming you are working on a traditional _[ASP.NET application model](https://docs.microsoft.com/en-us/aspnet/core/fundamentals/?view=aspnetcore-5.0&tabs=windows)_, you can start using the notification functions of this framework through the _Dependency Injection_ (DI) pattern, including a default implementation of the sender during the Startup of your application, by calling the `.AddWebhooks()` extension method provided by _Deveel Webhooks_, mixing the configurations of managament and delivery.

``` csharp
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
            services.AddWebhooks(webhooks => {
                // This registers a subscription resolver that is backed
                // by the subscription manager
                webhooks.UseSubscriptionManager();

                // for this implementation we use a Mongo database as
                // persistent layer of the subscriptions
                wekhooks.UseMongoDb(mongo => {
                    mongo.SetConnectionString(Configuration
                         .GetConnectionString("MongoWebhooks"))
                         .SetDatabase("example_app")
                         .SetSubscriptionsCollection("webhook_subs")
                         // Optional: enables the storage the webhook delivery results
                         .SetWebhooksCollection("webhooks_results");
                });

                // Optional: add a filter engine that handles string-based "linq" filters
                webhooks.AddDynamicLinqFilters();

                // Optional: configure the delivery behavior
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

After this you will have an instance of `IWebhookNotiier` instance that can be used witin your code for triggering the process described above.

``` csharp

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
            // The service requires an instance of EventInfo
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

## Basic Usage - Inbound (Receivers)


## Further Reading

Soon I will provide more advanced use cases of the framework, such as _extensions of the sender_, _custom subscription management_, _implementing data transformations_, etc.

## Contribute

If you wish to contribute, through _bug fixing_, new features or components, please feel free to open PRs or drop an e-mail to _antonello at deveel dot com_ for more information.

## License Information

This project is released under the [Apache 2 Open-Source Licensing agreement](https://www.apache.org/licenses/LICENSE-2.0).
