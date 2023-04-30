# SendGrid Webhook and E-Mail Receiver

The framework provides a set of configurations and extensions to support the capabilities for receiving and processing SendGrid webhooks and e-mails.

## Installation

You can install the package from NuGet, running the following command in the console:

```bash
dotnet add package Deveel.Webhooks.Receiver.SendGrid
```

## Configuration

### Receiving Webhooks

To activate the SendGrid receiver you don't need many cerimonies or configurations, just add the following line to the `ConfigureServices` method of your `Startup` class (assuming you are using a classic ASP.NET Core application):

```csharp
public void ConfigureServices(IServiceCollection services) {
  // ...
  services.AddSendGridReceiver()
	.AddHandler<SendGridWebhookHandler>();
  // ...
}
```

The above line will register the required services and configurations to the DI container, using the default configurations, so that the receiver can be used in the application.

If you need to customize directly the configurations, you can use the following overload of the `AddSendGridReceiver` method:

```csharp
public void ConfigureServices(IServiceCollection services) {
  // ...
  services.AddSendGridReceiver(options => {
	options.VerifySignatures = true;
	options.Secret = "my-secret";
  });
  // ...
}
```

If your configurations reside in a configuration section of the `appsettings.json` file, you can use the following overload:

```csharp
public void ConfigureServices(IServiceCollection services) {
  // ...
  services.AddSendGridReceiver("Webhooks:SendGrid")
	.AddHandler<SendGridWebhookHandler>();
  // ...
}
```

_Note: the above overload will use the `Webhooks:SendGrid` section of the configuration file to load the configurations, but any can be used_

### Receiving E-Mails

SendGrid and other providers of e-mail services support the capability to forward e-mails to a specific endpoint, so that the application can process them, using alternative methods than the classic SMTP protocol.

By nature, these HTTP requests are not considered as webhooks, but they are still HTTP requests that can be processed by the framework: in fact the receiver library provides a specific handler that can be used to process e-mails.

Since thes e-mails are not following the practices of webhooks, the receiver will not validate the signature of the request, but it will process it as it is, requiring no additional configurations.

To activate the e-mail receiver, you can use the following overload of the `AddSendGridEmailReceiver` method:

```csharp
public void ConfigureServices(IServiceCollection services) {
  // ...
  services.AddSendGridEmailReceiver("/email/sendgrid")
	.AddHandler<SendGridEmailHandler>();
  // ...
}
```


## Mapping Webhook Events

To map the events received from SendGrid to the handlers provided by the framework, you can use the `MapSendGridWebhook` and `MapSendGridEmail` extension methods of the `IApplicationBuilder` contract:

```csharp
public void Configure(IApplicationBuilder app, IHostingEnvironment env) {
  // ...
  app.MapSendGridWebhook("/webhook/sendgrid");
  app.MapSendGridEmail("/email/sendgrid");

  app.MapSendGridWebhook("/webhook/sendgrid/handled", webhook => {});

  app.MapSendGridEmail("/email/sendgrid/handled", email => {});
  // ...
}
```

The framework will bind the incoming webhooks and emails to instances of the `SendGridWebhook` and `SendGridEmail` classes, that can be used to process the data received from the provider.