# Producing Custom Data Factories

In automated scenarios of notification, when an event triggers a process of construction of the webhooks and their delivery to subscribers, there could be a need to _transform_ the original data carried by those triggering events.

In fact, sometimes events don't carry all information that have to be transmitted to the webhook receivers, for several reasons (privacy, design, external context, etc.), and this requires an additional intervention for the integration of those information.

_Note_: This is not a mandatory passage in the notification process, and can be skipped if the data carried by the event represents the information to be transferred to the receivers.

## Event Information

The overall contract of an event as recognized by the system is defined by the `EventInfo` structure, that is composed of the following fields:

  * `Id` - The event identifier
  * `Source` - The source of the event (e.g. `github`)
  * `Subject` - The subject of the event (e.g. `issue`)
  * `Type` - The type of the event (e.g. `created`)
  * `TimeStamp` - The exact time the event has occurred in the source system
  * `Data` - The payload that contains the actual data of the event

This design respect a general contract of the domain-driven design, informing of the event's nature and the payload that contains the actual data.

## Event Data

As mentioned before, the event data is not always in the format that is expected by the target system that will be notified, and the resons to implement a further passage of transformation are various:

* The event data might contain sensitive information, that should not be exposed to the target system
* The event data might contain information that is not relevant to the target system
* The event data might contain information that is not available in the event, but must be retrieved from external sources

## Transformation of Data

The notifier service uses the `IWebhookDataFactory` interface to transform the event data, that is received from the event bus, into the format that is expected by the target system, running all the registered transformations in a pipeline, until obtaining a version of the event that is suitable for the target system.

Data transformations are specific to your use cases and they can be specific to a given event condition (eg. _only a specific type of event with a given value in its 'data' component triggers the transformation_).

To implement this logic in the application, you must first create a new class that inherits from the `IWebhookDataFactory` contract.

```csharp
public class UserDataFactory : IWebhookDataFactory
{
    private readonly IUserResolver resolver;

	public MyWebhookDataFactory(IUserResolver resolver)
	{
		this.resolver = resolver;
	}

	public bool Handles(EventInfo eventInfo)
	{
		// Check if the event is handled by this factory
		return eventInfo.Source == "my-source" && 
			eventInfo.Subject == "person" && 
			eventInfo.Type == "created";
	}

	public Task<object> CreateData(EventInfo eventInfo, CancellationToken cancellationToken)
	{
		// Resolve the event data
		var userId = eventInfo.Data.GetString("userId");
		var user = await resolver.GetUserAsync(userId, cancellationToken);
		
		// Transform the event data
		return new UserCreatedData
		{
			Id = user.Id,
			Email = user.Email,
			FirstName = user.FirstName,
			LastName = user.LastName
		};
	}
}

```

**Note**: In the transformation pipeline of the notifier, the `IWebhookDataFactory` implementations are executed in the order they are registered in the container, and the event passed as argument of the data creation method is the result of the previous execution: only the data part of the event is applied to the event, while the other fields (e.g. `Id`, `Source`, `Subject`, `Type`, `TimeStamp`) are preserved.

## Registering the Data Factory

To enable the implemented by your data factory, you can register it during the configuration of the application.

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
                // This call registers the data factory as a
                // scoped service by default, but other overloads
                // allow controlling the lifetime
                webhooks.AddDataFactory<UserDataFctory>();
            });
        }
    }
}

```
