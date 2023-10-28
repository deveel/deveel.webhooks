# Getting Started

The overall design of this framework is open and extensible (implementing the traditional [Open-Closed Principle](https://en.wikipedia.org/wiki/Open%E2%80%93closed\_principle)), which means base contracts can be extended, composed, or replaced.

It is possible to use its components as they are provided or use the base contracts to extend single functions, while still using the rest of the provisioning.

### Sending and Receiving

The framework provides two major capabilities to the applications using its libraries

<table><thead><tr><th width="209.5">Capability</th><th>Description</th></tr></thead><tbody><tr><td><a href="notifications/"><strong>Notify Webhooks</strong></a></td><td>Communicate the occurrence of an event in a system to an external application that is listening for those events </td></tr><tr><td><a href="receivers/"><strong>Receive Webhooks</strong></a></td><td>Accepts and processes a notification from an external system, to trigger any related process</td></tr></tbody></table>

The two capabilities are disconnected from one other since they represent two different parts of the communication channel (the _Sender_ and the _Receiver_): as such the architecture of the framework is designed so that they don't depend on each other's.

##





## Receiving Webhooks

The framework also provides a set of services that can be used to receive webhooks from external systems, and to process them.

To do so, you need to add the `Deveel.Webhooks.Receiver.AspNetCore` library to your project, that must be an _ASP.NET Core_ application.

You can add the library to your project using the `dotnet` command line tool:

```bash
dotnet add package Deveel.Webhooks.Receiver.AspNetCore
```

or add the following line to your `csproj` file:

```xml
<PackageReference Include="Deveel.Webhooks.Receiver.AspNetCore" Version="1.1.6" />
```

### Configuring the Receiver

The receiver is configured using the `AddWebhooksReceiver` extension method on the `IServiceCollection` interface.

To run the receiver, you need to add the `UseWebhooksReceiver` middleware to the pipeline of your application, specifying the path where the receiver will be listening for the incoming webhooks.

When the service is configured with a webhook receiver, this will be invoked when a webhook is received, allowing the processing of the incoming webhook.

```csharp
using System;

using Microsoft.Extensions.Configuration;

using Deveel.Webhooks;

namespace Example {
	public class Startup {
		public Startup(IConfiguration configuration) {
			Configuration = configuration;
		}

		public IConfiguration Configuation { get; }

		public void Configure(IServiceCollection services) {
			services.AddWebhooksReceiver<MyWebhook>()
              .AddHandler<MyWebhookHandler>();
		}
        
        public void Configure(IApplicationBuilder app) {
            app.UseRouting();
            
            app.UseWebhooksReceiver("/my-webhook");
		}
	}
}
```

The `AddWebhooksReceiver` method accepts a generic type parameter that specifies the type of the webhook that will be received by the receiver: this allows to accept and process multiple webhooks in the same application.

### Handling Webhooks

The receiver will invoke the registered handlers in the order they are registered, that allows to process the incoming webhook in a pipeline.

Handlers can use dependency injection to access the services registered in the application.

```csharp
using System;

using Deveel.Webhooks;

namespace Example {
	public class MyWebhookHandler : IWebhookHandler<MyWebhook> {
        private readonly ILogger<MyWebhookHandler> logger;

        public MyWebhookHandler(ILogger<MyWebhookHandler> logger) {
			this.logger = logger;
		}

		public Task HandleAsync(MyWebhook webhook, CancellationToken cancellationToken) {
            logger.LogInformation("Received webhook {0}", webhook.Id);

			// Do something with the webhook
		}
	}
}
```
