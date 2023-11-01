# Receivers

## Receiving Webhooks

The framework also provides a set of services that can be used to receive webhooks from external systems and to process them.

To do so, you need to add the `Deveel.Webhooks.Receiver.AspNetCore` library to your project, which must be an _ASP.NET Core_ application.

You can add the library to your project using the `dotnet` command line tool:

```bash
dotnet add package Deveel.Webhooks.Receiver.AspNetCorebas
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



The framework provides a set of libraries that can be used to receive webhooks from external sources.

| Receiver                              | Description                                            |
| ------------------------------------- | ------------------------------------------------------ |
| [**Facebook**](facebook\_receiver.md) | Receive webhooks from Facebook Messenger               |
| [**SendGrid**](sendgrid\_receiver.md) | Receive webhooks and emails from SendGrid              |
| [**Twilio**](twilio\_receiver.md)     | Receive webhooks and SMS/WhatsApp messages from Twilio |
