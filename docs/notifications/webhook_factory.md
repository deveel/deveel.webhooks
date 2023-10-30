# Webhook Factories

In notification scenarios triggered by an event, the webhooks to be notified must be built and delivered to subscribers, and this construction process might need to _transform_ the original data carried by those triggering events into a new object.

In fact, sometimes events don't carry all information that have to be transmitted to the receivers, for several reasons (eg. _privacy_, _design_, _external context_, etc.), and this requires an additional intervention for the integration of those information (eg. _resolving an entity from the database_, _appending the environment variables of the notifier_, etc.).

_Note_: This is not a mandatory passage in the notification process, and can be skipped if the data carried by the event represents the information to be transferred to the receivers.

## Event Information

The overall contract of an event as recognized by the system is defined by the `EventInfo` structure, that is composed of the following fields:

| Field | Type | Description |
|-------|------|-------------|
| `Id` | `string` | The identifier of the event |
| `Source` | `string` | The source of the event (eg. `github`) |
| `Subject` | `string` | The subject of the event (eg. `issue`) |
| `Type` | `string` | The type of the event (eg. `created`) |
| `TimeStamp` | `DateTimeOffset` | The exact time the event has occurred in the source system |
| `Data` | `object` | The payload that contains the actual data of the event |

This design respects a general contract of the domain-driven design, informing of the event's nature and the payload that contains the actual data.

### Cloud Events

An example of the implementation of this contract is the [Cloud Events](https://cloudevents.io/) specification, that defines a standard for the representation of events in a cloud-native environment.

Anyway, despite the adherence to the overall design, the Deveel Webhooks framework is not tied to this specification, but it can be used to implement it, an it requires the EventInfo structure to be provided in order to be able to send notifications.

## Event Data

As mentioned before, the event data is not always in the format that is expected by the target system that will be notified, and the resons to implement a further passage of transformation are various:

* The event data might contain sensitive information, that should not be exposed to the target system
* The event data might contain information that is not relevant to the target system
* The event data might contain information that is not available in the event, but must be retrieved from external sources (eg._database entries_,	_environment variables_, etc.)

## Transforming the EventInfo to a Webhook: the `IWebhookFactory<TWebhook>` interface

The notifier service uses instances of the `IWebhookFactory<TWebhook>` interface to transform a triggering event, into a webhook object that is expected by the subscribing applications, using a subscription-scoped transformation logic.

Transformations are specific to your use cases and they can be specific to a given event condition (eg. _only a specific type of event with a given value in its 'data' component triggers the transformation_).

To implement this logic in the application, you must first create a new class that inherits from the `IWebhookFactory<TWebhook>` contract.

```csharp
public class MyWebhookFactory : IWebhookFactory<MyWebhook> {
    private readonly IUserResolver resolver;

	public MyWebhookFactory(IUserResolver resolver) {
		this.resolver = resolver;
	}

	public Task<MyWebhook> CreateAsync(IWebhookSubscription subscription, EventInfo eventInfo, CancellationToken cancellationToken) {
		// Resolve the event data
		var userId = eventInfo.Data.GetString("userId");
		var user = await resolver.GetUserAsync(userId, cancellationToken);

        var webhookType = $"user.{eventInfo.Type}";
        
        // Transform the event data
        return new MyWebhook {
            Type = webhookType,
            User = new UserInfo {
                Id = user.Id,
                Email = user.Email,
                FirstName = user.FirstName,
                LastName = user.LastName
            }
        };
    }
}
```

## Registering the Webhook Factory

To enable the implemented by your Webhook Factory, you can register it during the configuration of the application.

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
            services.AddWebhookNotifier(webhooks => {
                // This call registers your custom webhook factory as a
                // singleton service by default, but other overloads
                // allow controlling the lifetime
                webhooks.UseWebhookFactory<MyWebhookFctory>();
            });
        }
    }
}
```

## The Default WebhookFactory

When registering the Webhook Notifier service, by default the framework will register a default implementation of the `IWebhookFactory<TWebhook>` interface, if the type of `TWebhook` is derived from the `Webhook` class: this factory will attempt to create instances of the webhook type using the default constructor, and then it will try to map the properties of the webhook object to the properties of the event data and the subscription data.

This approach is useful when the webhook object is a simple POCO that can be easily mapped to the event data, and it doesn't require any further transformation logic, but it's not suitable for more complex scenarios.