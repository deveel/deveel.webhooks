# Filtering Webhook Subscriptions

By design, webhook subscriptions are bound to a set of _event types_, and they are resolved on the occurrence of one or more of the events matching the type subscribed.

It is possible to define _second-level filtering_ on the subscription, applying filters that will be evaluated, for the notification to be delivered to the subscriber.

Such capability is useful for scenarios like

* Avoid sending unnecessary notifications to the receiver
* &#x20;Reduce the load on the receiver
* Routing the delivery of the notifications to different receivers

## Webhook Filter Evaluators

Webhook subscriptions might include additional filters, such as IWebhookSubscriptionFilter, specifying the format in which they are expressed: a matching service supporting that format must be present in the application, for the evaluation to be performed.

These filtering conditions are evaluated by services implementing the `IWebhookFilterEvaluator` interface, that is in fact a filtering engine.

By default, when no filtering service is registered in the application to support a specific format of the webhook subscription's filter, the notification service will fail, and will not deliver the notification to the receiver.

#### Registering the Filter Service

To enable the filtering capability, the `IWebFilterEvaluator` service must be registered through the notification service builder:

```csharp
namespace Example {
    public class Startup {
        public void ConfigureServices(IServiceCollection services) {
            services.AddWebhookNotifier<MyWebhook>(webhooks => {
                // Register the filter evaluator service that
                // is using the "linq" syntax
                webhooks.UseDynamicLinq();
            });
        }
    }
}
```

In the above code, we registered the `DynamicLinqFilterEvaluator`, which is a service provided in the [Deveel.Webhooks.DynamicLinq](https://www.nuget.org/packages/Deveel.Webhooks.DynamicLinq) package and that uses the [DynamicLINQ](https://dynamic-linq.net/) syntax to evaluate filters.

### Evaluating Webhooks

The filtering engine is invoked with the instance of the webhook object to be delivered, and the filtering expressions are evaluated against its structure and data: keep in mind this when creating the filtering conditions.

The representation of the webhook is dependent on the implementation of the serialization service (eg. `System.Text.Json`, `Newtonsoft.Json` or `System.Xml`) and the format of the webhook payload (either `json` or `xml`): these serializer might have different behaviors when serializing the webhook object (such as attributes or properties, or the casing of the names), and the filtering conditions must be defined accordingly.

## LINQ Filters

As mentioned above, the framework provides the `DynamicLinqFilterEvaluator` service, which provides filtering capabilities using the LINQ syntax, a very powerful and flexible syntax to define filtering conditions.

You can install it by calling the following command on the root of your project:

```bash
dotnet add package Deveel.Webhooks.DynamicLinq --version 2.1.5
```

To enable it you will have to invoke the `.UseDynamicLinqFilters()`, like in the example above.

#### Example Filter

For example, consider a webhook that is serialized as a JSON object as the following one:

```json
{
  "event_type": "user.created",
  "user_name": "antonello",
  "user_email": "antonello@deveel.com",
  "roles": ["admin", "user"],
  "timestamp": 1682197154054
}
```

The filtering expression

```csharp
event_type == "user.created" && user_name.startsWith("anto")
```

will evaluate to `true` if the webhook is for the event `user.created` and the user name starts with `anto`.

**Note** - The engine does not provide any access to an external context other than the webhook object itself: this means that is not possible to access external data or services to evaluate the filtering conditions.
