# Webhook Receivers

The ability to receive webhooks is a core feature of the platform: webhooks are sent to your application when certain events occur in the platform. 

For example, when a user is created in an external system, a webhook is sent to your application with the details of the user. You can then use this information to create the user in your application.

## ASP.NET Receivers

The framework provides provides an implementation of a webhook receiver for ASP.NET Core applications, available as a NuGet package: [Webhook.Receiver.AspNetCore](https://www.nuget.org/packages/Webhook.Receiver.AspNetCore/).

You can use the contracts and the middlewares provided by the package to receive webhooks in your ASP.NET Core application and react to them, accordingly with the design of your application.

### Installation

To install the package, use the following command in the Package Manager Console:

```powershell
Install-Package Webhook.Receiver.AspNetCore
```

or use the .NET CLI:

```bash
dotnet add package Webhook.Receiver.AspNetCore
```

## Instrumenting the Application

To start receiving webhooks in your ASP.NET Core application, you need to register the webhook receiver in the service collection, and add the webhook receiver middleware to the application pipeline.

The following code shows how to register the webhook receiver in the service collection, and how to add the webhook receiver middleware to the application pipeline:

```csharp
namespace Example {
	public class Startup {
		public void ConfigureServices(IServiceCollection services) {
			services.AddWebhookReceiver<IdentityWebhook>()
			    .AddHandler<UserRegisteredHandler>();
		}
	}

	public void Configure(IApplicationBuilder app, IWebHostEnvironment env) {
	    // Use the registered factory handlers ...
		app.MapWebhook<IdentityWebhook>("/ids/webhooks/");

		// ... or use a middleware to handle the webhook
		app.MapWebhook<IdentityWebhook>("/ids/webhooks/handled", async(IdentityWebhook webhook, ILogger<IdentityWebhook> logger) => {
			// Handle the webhook
			logger.LogInformation("Webhook received: {Webhook}", webhook);
		});
	}
}
```

## Factory-Based Handlers

The framework provides two alternative methods to handle webhooks, depending on the design of your application or the complexity of the webhook handling logic.

The first method is to uses a factory to create the handlers registered in the service collection, and it's the most suitable for scenarios where to handle a webhook you need to depend on one or more external services.

For example, consider the following webhook handler:

```csharp
namespace Example {
	public class UserCreatedHandler : IWebhookHandler<IdentityWebhook> {
		private readonly IUserService _userService;

		public UserCreatedHandler(IUserService userService) {
			_userService = userService;
		}

		public async Task HandleAsync(IdentityWebhook webhook, CancellationToken cancellationToken) {
			var userInfo = webhook.Data.UserInfo;

			var user = new User {
				Email = userInfo.Email,
				FirstName = userInfo.FirstName,
				LastName = userInfo.LastName,
				ExternalId = userInfo.Id
			};

			await _userService.CreateUserAsync(user, cancellationToken);
		}
	}
}
```

When creating the webhook receiver, you can register the handler in the service collection using the service builder, and wire the handler to the webhook type:

```csharp
namespace Example {
	public class Startup {
		public void ConfigureServices(IServiceCollection services) {
			services.AddWebhookReceiver<IdentityWebhook>()
				.AddHandler<UserCreatedHandler>();
		}
	}

	public void Configure(IApplicationBuilder app, IWebHostEnvironment env) {
		app.MapWebhook<IdentityWebhook>("/ids/webhooks/");
	}
}
```

The above code works as follow:

1. The `AddWebhookReceiver` method registers the webhook receiver in the service collection, isolating any behavior to the webhook type `IdentityWebhook`
2. The `AddHandler` method registers the handler in the service collection as a scoped service
3. The `MapWebhook` method maps the specific path (for a `POST` request) to the webhook receiver middleware, that receives webhooks of type `IdentityWebhook`
4. When a webhook is received, the middleware will create a scope and resolve any handlers associated to the webhook of type `IdentityWebhook`, passing them the webhook to handle

### Webhook Handling

When a webhook is received and the handlers are resolved, their execution is performed in parallel by default, and the middleware will wait for the completion of all the handlers before returning a response to the sender.

It is recommended that implementations of the handlers are designed to be executed in a non-blocking form, to avoid blocking the middleware and the sender of the webhook: currently no background process is executed to handle the webhooks, and the middleware will wait for the completion of all the handlers before returning a response to the sender.

### Execution Modes

By default, the middleware will execute all the registered the handlers (fo the type of webhook) in parallel.

This behavior can be changed by specifying an execution mode when registering the webhook receiver, using the `ExecutionMode` configuration property of the `WebhookHandlingOptions` class, when calling the `MapWebhook` method.

```csharp
namespace Example {
	public class Startup {
		public void ConfigureServices(IServiceCollection services) {
			services.AddWebhookReceiver<IdentityWebhook>()
				.AddHandler<UserCreatedHandler>();
		}
	}
	public void Configure(IApplicationBuilder app, IWebHostEnvironment env) {
		app.MapWebhook<IdentityWebhook>("/ids/webhooks/", new WebhookHandlingOptions {
			ExecutionMode = WebhookExecutionMode.Sequential;
		});
	}
}
```

## Convention-Based Receivers

Another method to handle webhooks is to use middlewares to handle webhooks, and it's the most suitable for scenarios where the handling of the webhook is simple and doesn't require to depend on several external services (for example, when using a mediator to handle the webhook).

This is done by passing a delegate to the `MapWebhook` method, that will be directly invoked by the middleware when a webhook is received, without attempting to resolve any further handler for the same type of webhook.

```csharp
namespace Example {
	public class Startup {
		public void ConfigureServices(IServiceCollection services) {
			services.AddWebhookReceiver<IdentityWebhook>();
		}
	}
	public void Configure(IApplicationBuilder app, IWebHostEnvironment env) {
		app.MapWebhook<IdentityWebhook>("/ids/webhooks/", async (IdentityWebhook webhook, CancellationToken  cancellationToken) => {
			var mediator = context.RequestServices.GetRequiredService<IMediator>();
			await mediator.Send(new CreateUserCommand(webhook.Data.UserInfo), cancellationToken);
		});
	}
}
```

As you can see the above code is simpler than the previous one, but it comes with a limitation: the first argument must always be the webhook of the type handled by the middleware. 

One of the arguments (in no particular position) can be a cancellation token, that can be used to cancel the execution of the middleware: this will be the same cancellation token used by the middleware to cancel the execution of the handlers.

This method provides few alternative signatures to the delegate, depending on the design of your application, that can be executed synchronously or asynchronously.

The following code shows the alternative signatures of the delegate:

```csharp
// Async
MapWebhook<TWebhook>(string path, Func<TWebhook, Task> handler);
MapWebhook<TWebhook, T1>(string path, Func<TWebhook, T1, Task> handler);
MapWebhook<TWebhook, T1, T2>(string path, Func<TWebhook, T1, T2, Task> handler);
MapWebhook<TWebhook, T1, T2, T3>(string path, Func<TWebhook, T1, T2, T3, Task> handler);

// Sync
MapWebhook<TWebhook>(string path, Action<TWebhook> handler);
MapWebhook<TWebhook, T1>(string path, Action<TWebhook, T1> handler);
MapWebhook<TWebhook, T1, T2>(string path, Action<TWebhook, T1, T2> handler);
MapWebhook<TWebhook, T1, T2, T3>(string path, Action<TWebhook, T1, T2, T3> handler);
```

Any additional parameter than the webhook will be resolved in the request scope, and passed to the delegate when invoked.

### Webhook Handling

When using the delegate-based method to handle webhooks, the middleware will invoke the given delegate when a webhook is received, and will wait for the completion of the delegate before returning a response to the sender.

It is recommended that implementations of the delegate are designed to be executed in a non-blocking form, to avoid blocking the middleware and the sender of the webhook: currently no background process is executed to handle the webhooks, and the middleware will wait for the completion of the delegate before returning a response to the sender.

## Webhook Types

The overall design of the framework allows the segregation of the receiving functions to the webhook type, so that you can register multiple webhook receivers in the same application, each one handling a different type of webhook.

Consider for example the following code:

```csharp
namespace Example {
	public class Startup {
		public void ConfigureServices(IServiceCollection services) {
			services.AddWebhookReceiver<IdentityWebhook>()
				.AddHandler<UserCreatedHandler>();
			services.AddWebhookReceiver<PaymentWebhook>()
				.AddHandler<PaymentCreatedHandler>();
		}
	}

	public void Configure(IApplicationBuilder app, IWebHostEnvironment env) {
		app.MapWebhook<IdentityWebhook>("/ids/webhooks/");
		app.MapWebhook<PaymentWebhook>("/payments/webhooks/");
	}
}
```

The above code registers two webhook receivers, one for the `IdentityWebhook` type and one for the `PaymentWebhook` type, and it registers a handler for each webhook type.

This allows separating the behaviors in configuring and handling the webhooks, which might come from different sources and have different payloads.