# Facebook Webhook Receiver

This is a simple webhook receiver for Facebook Messenger. It is designed to be used with [Facebook Messenger Platform](https://developers.facebook.com/docs/messenger-platform).

## Installation

To install the package in your project, use the following command:

```bash
dotnet add package Deveel.Webhooks.Receiver.Facebook
```

## Instrument the Webhook Receiver

To enable your application to receive webhooks and messages from Facebook Messenger, you need to register the receiver and then wire it up to your application.

The following example shows how to register the receiver in a ASP.NET Core application:

```csharp
public void ConfigureServices(IServiceCollection services) {
	// ...
	services.AddFacebookReceiver()
	    .AddHandler<FacebookMessageReceivedHandler>();
	// ...
}
```

The following example shows how to wire up the receiver in a ASP.NET Core application:

```csharp
public void Configure(IApplicationBuilder app, IHostingEnvironment env) {
	// ...

	app.MapFacebookWebhook("/webhook/facebook");
	app.MapFacebookVerify("/facebook/verify");
	// ...
}
```

The `MapFacebookWebhook` extension method is used to map the webhook endpoint in the application pipeline, that will be used by Facebook to send webhooks and messages.

The `MapFacebookVerify` extension method is used to map the endpoint used by Facebook to verify that your application is authorized to receive webhooks.


## Configuration

The receiver can be configured using the `FacebookReceiverOptions` class, that can be passed to the `AddFacebookReceiver` method during the registration process.

The following code shows how to configure the receiver:

```csharp
public void ConfigureServices(IServiceCollection services) {
    // ...
    services.AddFacebookReceiver(options => {
        options.AppSecret = Configuration["Facebook:AppSecret"],
        options.VerifyToken = Configuration["Facebook:VerifyToken"]
        options.VerifySignature = true
    });
}
```

As you can notice, the set of configurations provided by the `FacebookReceiverOptions` class are less than the one available from the `WebhookReceiverOptions` class, because the _Facebook Messenger Platform_ has a more strict set of requirements for the webhook receiver.

The following table shows the available options for the receiver:

| Option | Description |
|--------|-------------|
| `AppSecret` | The application secret provided by Facebook |
| `VerifyToken` | The token used to verify the webhook endpoint |
| `VerifySignature` | A flag to indicate if the receiver should verify the signature of the incoming messages |

