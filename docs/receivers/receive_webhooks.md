# Receiving Webhooks

The `Deveel.Webhooks.Receiver` library provides a set of components that can be used to receive webhooks from external sources, and handle them in an ASP.NET Core application.

The following sections describe how to use the library to receive custom webhooks in an ASP.NET Core application, and how to handle them, but other _out-of-the-box_ implementations are also provided for specific providers (eg. Facebook, Twilio, SendGrid, etc.): please check the specific section for the configuration of your application to receive webhooks from those providers.

## Installation

To start receiving webhooks from external sources, you can use the `Deveel.Webhooks.Receiver.AspNetCore` library, that allows the registration of a webhook receiver in an ASP.NET Core application.

Run this command on the root of your project to install the library from NuGet:

```bash
dotnet add package Deveel.Webhooks.Receiver.AspNetCore
```

## Instrumenting the Application

If you are using a traditional ASP.NET Core MVC application, you can register the webhook receiver service by modifying the `Startup` class as follows:

```csharp
public class Startup {
  public void ConfigureServices(IServiceCollection services) {
	services.AddWebhookReceiver<MyWebhook>();
  }
}
```

Alternatively, if you are using the mininal API pattern, you can use the following code:

```csharp
var builder = WebApplication.CreateBuilder(args);

builder.Services.AddWebhookReceiver<MyWebhook>();
```

The above simple calls registers the webhook receiver in the application, and enable it to receive webhooks of type `MyWebhook`: receivers are segregated by the type of webhook they can handle, and you can register multiple receivers for different types of webhooks.

By default the registration of the webhook receiver adds a set of default services, that are required to handle the webhooks, such as the `IWebhookReceiver<MyWebhook>` and `IWebhookHandler<MyWebhook>`, and a default set of options: you can control further the services and configurations by using the builder instance returned by the `AddWebhookReceiver` method.

## Receiving Webhooks - Using Controllers

Following the registration of the webhook receiver, you can receive webhooks by using the `IWebhookReceiver<MyWebhook>` service, that is registered in the application, if you want to handle the receive process directly.

This approach is typical in MVC APIs that implement the request processing in the controller, and can be used as follows:

```csharp
namespace Demo {
  [ApiController]
  [Route("webhook")]
  public class WebhookController : ControllerBase {
	private readonly IWebhookReceiver<MyWebhook> webhookReceiver;
	private readonly IWebhookHandler<MyWebhook> webhookHandler;

	public WebhookController(IWebhookReceiver<MyWebhook> webhookReceiver, IWebhookHandler<MyWebhook> webhookHandler) {
	  this.webhookReceiver = webhookReceiver;
	  this.webhookHandler = webhookHandler;
	}

	[HttpPost]
	public async Task<IActionResult> ReceiveWebhook() {
	  var result = await webhookReceiver.ReceiveAsync(Request, HttpContext.RequestAborted);
	  if (!result.IsValid)
		return BadRequest(result.Error);

		var webhook = result.Webhook;
		await webhookHandler.HandleAsync(webhook, HttpContext.RequestAborted);

	  return Ok();
	}
  }
}
```

Mind that in the above scenario you must also inject the `IWebhookHandler<MyWebhook>` service, that is used to handle the received webhook.

**Note** - The design of the receiver allows the registration of multiple handlers for the same type of webhook, which can be injected in the controller as an `IEnumerable<IWebhookHandler<MyWebhook>>` service, and can be used to handle the webhook in different ways. For simplicity of the example, we are using a single handler.

## Receiving Webhooks - Using Middlewares

Alternatively the `Deveel.Webhooks.Receiver.AspNetCore` library provides a middleware that can be used to receive webhooks, and handle them automatically.

To use the middleware, you must first register it in the `Startup` class of your application:

```csharp
public class Startup {
  public void ConfigureServices(IServiceCollection services) {
    services.AddWebhookReceiver<MyWebhook>();
  }

public void Configure(IApplicationBuilder app, IWebHostEnvironment env) {
	app.MapWebhook<MyWebhook>("/webhook");
  }
}
```

If you are using the minimal API pattern, you can use the following code:

```csharp

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddWebhookReceiver<MyWebhook>();

var app = builder.Build();

app.MapWebhook<MyWebhook>("/webhook");

app.Run();
```

The above code registers the middleware in the application, and enables the application to receive webhooks of type `MyWebhook` at the `/webhook` endpoint, using the configurations defined when registering the receiver,

The middlware will automatically scan for all the registered webhook receiver service configured, and will handle the received webhooks by invoking all the `IWebhookHandler<MyWebhook>` services registere.

The middleware design allows to handle the webhooks without any prior registered handler, by specifying an handling delegate in the `MapWebhook` method:

```csharp
[...]

app.MapWebhook<MyWebhook>("/webhook", async webhook => {
  // Handle the webhook here
  await Task.CompletedTask;
});
```

Or a alternatively a synchronous handling delegate:

```csharp
[...]

app.UseWebhookReceiver<MyWebhook>("/webhook", webhook => {
  // Handle the webhook here
});
```

## Further Reading

If you want to learn more and learn more advanced usage of the receivers, visit the [Advanced Usage of Receiver](custom\_receiver.md).
