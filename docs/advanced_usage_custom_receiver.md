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

## Factory Handlers

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
		app.UseWebhookReceiver<IdentityWebhook>("/ids/webhooks/");
	}
}
```

The above code works as follow:

1. The `AddWebhookReceiver` method registers the webhook receiver in the service collection, isolating any behavior to the webhook type `IdentityWebhook`
2. The `AddHandler` method registers the handler in the service collection as a scoped service
3. The `UseWebhookReceiver` method adds the webhook receiver middleware to the application pipeline, using the specified path as the endpoint to receive webhooks of type `IdentityWebhook`
4. When a webhook is received, the middleware will create a scope and resolve any handlers associated to the webhook of type `IdentityWebhook`, passing them the webhook to handle

## Middlewares Receivers

Another method to handle webhooks is to use middlewares to handle webhooks, and it's the most suitable for scenarios where the handling of the webhook is simple and doesn't require to depend on several external services.

This is done by passing a delegate to the `UseWebhookReceiver` method, that will be directly invoked by the middleware when a webhook is received, without attempting to resolve any further handler for the same type of webhook.

```csharp
namespace Example {
	public class Startup {
		public void ConfigureServices(IServiceCollection services) {
			services.AddWebhookReceiver<IdentityWebhook>();
		}
	}
	public void Configure(IApplicationBuilder app, IWebHostEnvironment env) {
		app.UseWebhookReceiver<IdentityWebhook>("/ids/webhooks/", async (context, webhook, cancellationToken) => {
			var mediator = context.RequestServices.GetRequiredService<IMediator>();
			await mediator.Send(new CreateUserCommand(webhook.Data.UserInfo), cancellationToken);
		});
	}
}
```

As you can see the above code is simpler than the previous one, but it's also less flexible: you can't use a factory to create the handler, and you can resolve external services only from the `HttpContext` instance.

This method provides few alternative signatures to the delegate, depending on the design of your application, that can be executed synchronously or asynchronously.

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
		app.UseWebhookReceiver<IdentityWebhook>("/ids/webhooks/");
		app.UseWebhookReceiver<PaymentWebhook>("/payments/webhooks/");
	}
}
```

The above code registers two webhook receivers, one for the `IdentityWebhook` type and one for the `PaymentWebhook` type, and it registers a handler for each webhook type.

This allows separating the behaviors in configuring and handling the webhooks, which might come from different sources and have different payloads.

## Webhook Handling

When a webhook is received and the handlers are resolved, their execution is performed in sequence, and the middleware will wait for the completion of all the handlers before returning a response to the sender.

It is recommended that implementations of the handlers are designed to be executed in a non-blocking form, to avoid blocking the middleware and the sender of the webhook: currently no background process is executed to handle the webhooks, and the middleware will wait for the completion of all the handlers before returning a response to the sender.

_**Note**: This design might change in the future, to allow the execution of the handlers in parallel, or in background in a non-blocking form._