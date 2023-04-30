# Twilio Webhook Receiver

Twilio is a cloud communications platform as a service (CPaaS) provider, allowing software developers to programmatically make and receive phone calls, send and receive text messages, and perform other communication functions using its web service APIs.

In the process of messaging, Twilio sends a webhook to a configured URL: this contains incoming messages, directed to the receiver, or the status of outgoing messages, sent by an application.

## Installation

To enable your ASP.NET Core application to receive Twilio Webhooks, install the Deveel.Webhooks.Receiver.Twilio library, using the NuGet package manager:

```bash
dotnet add package Deveel.Webhooks.Receiver.Twilio
```

## Configuration

To configure the Twilio Webhook Receiver, you need to add the `TwilioWebhookReceiver` to the services collection of your application, in the `ConfigureServices` method of the `Startup` class:

```csharp
public void ConfigureServices(IServiceCollection services) {
  // ...
  services.AddTwilioReceiver();
  // ...
}
```

Then, you need to configure the receiver in the `Configure` method of the `Startup` class:

```csharp
public void Configure(IApplicationBuilder app, IWebHostEnvironment env) {
  // ...
  app.MapTwilioWebhook("/twilio/webhook");

  app.MapTwilioWebhook("/twilio/other", webhook => {
    // ...
  });
  // ...
}
```