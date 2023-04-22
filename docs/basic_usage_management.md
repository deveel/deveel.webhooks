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

_**Note** - [05-2023] The packages provided by framework for the management of webhook subscriptions are going through a major redesign. the below specifications must be considered provisional_

---

# Webhook Subscription Management

While it's true that the simple process of sending _webhooks_ to recipient \[systems\], provided through implementations of the `IWebhookSender` service, is not dependent from the existence of any _Subscription Management_ capability, this is recomended to enable notification scenarios.

In fact, the notification of events to a recipient systems in a reactive mode requires the existence of a _Subscription_ to a certain type of event, and optionally other additional conditions (_subscription filters_), to ease the process of notification: users can subscribe to a certain type of event, and the system will send a notification to the recipient systems of the users only when the event occurs.

The Deveel.Webhooks framework provides a default implementation of the services and classes that can be used for the management and resolution of subscriptions, but you might want implement your own components to further ehnance the _Webhook Subscription Management_ capabilities of your application (see the section on _[Advanced Usage - Custom Webhook Management](advaned_usage_custom_management.md))_, using the remaining parts of the framework.

## Service Instrumentation

Assuming you are working on a traditional _[ASP.NET application model](https://docs.microsoft.com/en-us/aspnet/core/fundamentals/?view=aspnetcore-6.0&tabs=windows)_, you can start using the management functions of this framework through the _Dependency Injection_ (DI) pattern, including the base implementations during the Startup of your application, calling the `.AddWebhooks<MySubscription>()` extension method provided by _Deveel Webhooks_:

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
            services.AddWebhooks<MongoWebhookSubscription>(webhooks => {
                // for this implementation we use a Mongo database as
                // persistent layer of the subscriptions
                wekhooks.UseMongoDb(mongo => {
                    mongo.SetConnectionString(Configuration
                         .GetConnectionString("MongoWebhooks"))
                         .SetDatabase("example_app")
                         .SetSubscriptionsCollection("webhook_subs");
                });
            });
        }
    }
}

```

## Using the Mnagement Service

You can proceed by creating a new API controller and name it `WebhookSubscriptionController`, specifying one of the arguments of type `WebhookSubscriptionManager<MongoWebhookSubscription>` in the constructor, that you can use to manage the subscriptions:

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
