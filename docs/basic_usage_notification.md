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

# Webhook Notification

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

_**Note**: Since in this example no `IWebhookDataFactory` instance has been registered, the webhook payload will contain the same data transported by the event._

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
