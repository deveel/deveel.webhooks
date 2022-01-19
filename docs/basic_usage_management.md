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

# Webhook Subscription Management

The process of sending _webhooks_ to recipient \[systems\] (implemented by the `IWebhookSender` contract), at least in the model provided by _Deveel Webhooks_, is not technically dependent from the existence of the subscription management component: that means you might implement your own subscription management (see the section on _[Advanced Usage - Custom Webhook Management](advaned_usage_custom_management.md)_ for further information on how to do it).

In fact this requirement exists for the _notification_ scenarios (implemented by the `IWebhookNotifier`), in which the framework resolves the matching subscriptions to an event, forms the webhook and delivers it through the sender.

Anyway the framework provides an implementation for the management and resolution of subscriptions to certain type of events, and eventually other conditions (_filters_) to ease the process of notification.

## Service Instrumentation

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

You can proceed by creating a new API controller and name it `WebhookSubscriptionController`, specifying one of the arguments of type `IWebhookSubscriptionManager` in the constructor>

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
