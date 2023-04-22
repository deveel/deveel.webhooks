# Receiving Webhooks


## Installations

When receiving webhooks from external sources, you can use the `Deveel.Webhooks.Receiver.AspNetCore` library, that allows the registration of a webhook receiver in an ASP.NET Core application.

To enable this function you must first install the NuGet package:

```bash
dotnet add package Deveel.Webhooks.Receiver.AspNetCore
```


## Registering the Webhook Receiver

Then, in the `Startup` class of your application, you can register the webhook receiver as follows:

```csharp
public class Startup {
  public void ConfigureServices(IServiceCollection services) {
	services.AddWebhookReceiver<MyWebhook>();
  }
}
```

Or alternatively, if you are using the mininal API pattern, you can use the following code:

```csharp
var builder = WebApplication.CreateBuilder(args);

builder.Services.AddWebhookReceiver<MyWebhook>();
```

This simple call registers the webhook receiver in the application, and allows to receive webhooks of type `MyWebhook`: receivers are segregated by the type of webhook they can handle, and you can register multiple receivers for different types of webhooks.

By default the registration of the webhook receiver adds a set of default services, that are required to handle the webhooks, such as the `IWebhookReceiver<MyWebhook>`, `IWebhookHandler<MyWebhook>`, `IWebhookJsonParser<MyWebhook>` and a default set of options: you can control further the services and configurations by using the builder instance returned by the `AddWebhooks` method.

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

## Receiving Webhooks - Using Middleware

Alternatively the `Deveel.Webhooks.Receiver.AspNetCore` library provides a middleware that can be used to receive webhooks, and handle them automatically.

To use the middleware, you must first register it in the `Startup` class of your application:

```csharp
public class Startup {
  public void ConfigureServices(IServiceCollection services) {
    services.AddWebhooks<MyWebhook>();
  }

public void Configure(IApplicationBuilder app, IWebHostEnvironment env) {
	app.UseWebhooks<MyWebhook>("/webhook");
  }
}
```

If you are using the minimal API pattern, you can use the following code:

```csharp

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddWebhooks<MyWebhook>();

var app = builder.Build();

app.UseWebhooks<MyWebhook>("/webhook");

app.Run();
```

The above code registers the middleware in the application, and allows to receive webhooks of type `MyWebhook` at the `/webhook` endpoint, using the configurations defined when registering the receiver,

The middlware will automatically scan for all the registered webhook receivers, and will handle the received webhooks by using the `IWebhookHandler<MyWebhook>` service.

The middleware design allows to handle the webhooks without any prior registered handler, by specifying an handling delegate in the `UseWebhooks` method:

```csharp
[...]

app.UseWebhooks<MyWebhook>("/webhook", (context, webhook, cancellationToken) => {
  // Handle the webhook here
});
```

Or a alternatively a synchronous handling delegate:

```csharp
[...]

app.UseWebhooks<MyWebhook>("/webhook", (context, webhook) => {
  // Handle the webhook here
});
```