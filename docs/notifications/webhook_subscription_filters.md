# Filtering Webhook Subscriptions

To allow a webhook to be triggered only when a specific event occurs, subscriptions can define some filtering conditions that are evaluated before the webhook is delivered.

Such capability is useful to avoid sending unnecessary notifications to the webhook endpoint, and to reduce the load on the webhook endpoint, or for even implementing a routing mechanism to deliver the notifications to different endpoints.

## Filter Evaluators

The filtering conditions are evaluated by a set of services implementing the `IFilterEvaluator` interface, that is in fact a filtering engine. 

When no filtering service is registered, the notification service will not evaluate any filtering condition, and will deliver the notification to the webhook endpoint, even if the subscription defines some filtering conditions.

To enable the filtering capability, the `IFilterEvaluator` service must be registered through the notification service builder:

```csharp

namespace Example {
    public class Startup {
		public void ConfigureServices(IServiceCollection services) {
			services.AddWebhookNotifier(webhooks => {
				// Register the filter evaluator service that
				// is using the "linq" syntax
				webhooks.UseDynamicLinq();
			});
		}
	}
}
```

The filtering engine is invoked with the instance of the webhook to be delivered, and the filtering expressions are evaluated against its structure and data: keep in mind this when creating the filtering conditions.

The representation of the webhook is dependent on the implementation of the serialization service (eg. `System.Text.Json`, `Newtonsoft.Json` or `System.Xml`) and the format of the webhook payload (either `json` or `xml`): these serializer might have different behaviors when serializing the webhook object (such as attributes or properties, or the casing of the names), and the filtering conditions must be defined accordingly.

## LINQ Filters

Currently the framework provides the service `DynamicLinqFilterEvaluator`  that provides filtering capabilities using the LINQ syntax, that is a very powerful and flexible syntax to define filtering conditions.

For example, considering a webhook that is serialiazed as a JSON object as the following one:

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

```linq
event_type == "user.created" && user_name.startsWith("anto")
```

will evaluate to `true` if the webhook is delivered for the event `user.created` and the user name starts with `anto`.

**Note** - The engine does not provide any access to an external context than the webhook object itself, so it is not possible to access external data or services to evaluate the filtering conditions.