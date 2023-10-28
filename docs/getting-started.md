# Getting Started

The overall design of this framework is open and extensible (implementing the traditional [Open-Closed Principle](https://en.wikipedia.org/wiki/Open%E2%80%93closed_principle)), that means base contracts can be extended, composed or replaced.

It is possible to use its components as they are provided, or use the base contracts to extend single functions, while still using the rest of the provisioning.

## Intall the Required Libraries

The overall set of libraries are available through [NuGet](https://nuget.org), and can be installed and restored easily once configured in your projects.

At the moment (_November 2023_) they are developed as `.NET 6.0` and thus compatible with all the profiles of the .NET framework greater or equal than this.

The core library of the framework is `Deveel.Webhooks` and can be installed through the `dotnet` tool command line

```sh
$ dotnet add package Deveel.Webhooks
```

Or by editing your `.csproj` file and adding a `<PackageReference>` entry.

``` xml
<Project Sdk="Microsoft.NET.Sdk">

  <PropertyGroup>
    <TargetFramework>ne6.0</TargetFramework>
    ...

  </PropertyGroup>

  <ItemGroup>
    <PackageReference Include="Deveel.Webhooks" Version="2.1.1" />
    ...
  </ItemGroup>
</Project>
```

This provides all the functions that are needed to manage subscriptions and send webhooks to a given destination, activating the notification process (although this one would require external instrumentations, for resolving subscriptions and other advanced functions).

## Adding the Webhook Service

To begin using the functions of the webhook service, all that you need is to invoke the `.AddWebhooks()` function of the service collection in the instrumentation of your application: this will add the required default services to the container, enabling the application to start notifying events to the registered recipients.

For example, assuming you are working on a traditional _[ASP.NET application model](https://docs.microsoft.com/en-us/aspnet/core/fundamentals/?view=aspnetcore-6.0&tabs=windows)_, you can enable these functions like this:

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
            // using the default configurations
            services.AddWebhooks<MyWebhook>();
        }
    }
}

```

**Note**: To be able to notify events, you need to provide a type that implements the `IWebhookSubscriptionResolver` interface, that can be used to resolve the subscriptions for a given event. This is not provided by the core library by default, and you can add implementations that are specific for a given storage system (like the one provided by the `Deveel.Webhooks.MongoDb` library).

### Configuring the Delivery Behavior

The framework provides a default behavior for the delivery of webhooks to the registered recipients, but you can configure the behavior of the service to suit your needs.

The library `Deveel.Webhooks` depends from the `Deveel.Webhooks.Sender` library, which provides the needed functions to send webhooks to receivers: you will find the abstractions and helpers to configure the behavior of the webhook delivery to recipient systems, controlling aspects of the process like _payload formatting_, _retries on failures_, _signatures_.

You have several options to configure the service, and therefore you are free to chose the methodology that suits you best.

### The WebhookSenderOptions

The `WebhookSenderOptions` class provides a set of properties that can be used to configure the behavior of the webhook sender.

The configurations are evolving with the versions of the framework, and the following ones apply to the current version (`1.1.6`) of the library.

``` csharp

var options = new WebhookSenderOptions {
  // When using the IHttpClientFactory, this is the name of the client
  // that will be used to send the webhooks
  HttpClientName = "my-http-client",

  // A set of default headers that will be added to the requests
  // sent to the webhook recipients
  DefaultHeaders = new Dictionary<string, string> {
	// The default headers that will be added to the requests
	// sent to the webhook recipients
	["X-Sender"] = "My-Webhook-Sender/1.0"
  },

  // The default format of the payload that will be sent to the
  // webhook recipients (possible values are Json and Xml)
  DefaultFormat = WebhookPayloadFormat.Json,

  // The default timeout for the requests sent to the webhook
  // recipients to be completed, before being considered failed
  Timeout = TimeSpan.FromSeconds(30),

  // The default retry options for the requests sent to the webhook
  // recipients, that can be overridden by the specific subscription
  // configurations
  Retry = new WebhookRetryOptions {
    // The default number of retries that will be performed
    // after a failed request, before giving up
    MaxRetries = 3,
    
    // The default delay between retries
    Delay = TimeSpan.FromSeconds(5),

    // The default timeout for each retry request
    // before being considered failed
    Timeout = TimeSpan.FromSeconds(3)
  },
  
  // The default signature options for the requests sent to the webhook
  // recipients, that can be overridden by the specific subscription
  // configurations
  Singature = new WebhookSenderSignatureOptions {
    // The default location within the request where the signature
    // will be added (possible values are Header and QueryString)
    Location = WebhookSignatureLocation.Header,

    // The default name of the header that carries the signature,
    // when the location of the signature is the Header
    HeaderName = "X-Signature",
    
    // The default name of the query string parameter that carries
    // the signature, when the location of the signature is the QueryString
    QueryParameter = "signature",
    
    // The default algorithm that will be used to sign the requests
    // sent to the webhook recipients
    Algorithm = WebhookSignatureAlgorithm.HmacSha256,

    // The name of the query string parameter that will be used to
    // specify the algorithm used for the signature
    AlgorithmQueryParameter = "alg_sig"
  }
};

```

Mind that every subscription can override some of the default options, and you can also provide a custom implementation of the `IWebhookSender` interface to customize the behavior of the delivery process.

### IConfiguration Pattern

If you keep your configurations in an `appsetting.json` file, environment variables, secrets, implementing a typical pattern of the _ASP.NET_ applications, you can invoke the overloads provided by `Deveel.Webhook` that access the available instances of `IConfiguration`.

``` csharp
using System;

using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

using Deveel.Webhooks;

namespace Example {
    public class Startup {
        public Startup(IConfiguration configuration) {
            Configuration = configuration;
        }

        public IConfiguration Configuation { get; }

        public void Configure(IServiceCollection services) {
            // The configurations are specified in the 'Webhooks:Sender'
            // section of the configuration instance, and the service
            // will find the IConfiguration instance within the
            // container and use it to configure the service
            services.AddWebhooks<MyWebhook>("Webhooks:Sender");
        }
    }
}
```

After this, an instance of `IOptions<WebhookDeliveryOptions>` is available for injection in the webhook services or in your code.

Given the design of the service, it will also be possible to access a webhook-specific instance of `IOptions<WebhookDeliveryOptions>` for a given webhook type, using the `IOptionsSnapshot<TOptions>` service, using the type name of the webhook as the key (eg. `IOptionsSnapshot<WebhookSenderOptions>.Get("MyWebhook")`).

### Manual Configuration

If you prefer to configure the service manually, you can use the `AddWebhooks` overload that accepts an instance of `WebhookSenderOptions` as parameter.

``` csharp
using System;

using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

using Deveel.Webhooks;

namespace Example {
    public class Startup {
        public Startup(IConfiguration configuration) {
            Configuration = configuration;
        }

        public IConfiguration Configuation { get; }

        public void Configure(IServiceCollection services) {
            services.AddHttpClient("my-http-client");

            services.AddWebhooks<MyWebhook>(options => {
              options.HttpClientName = "my-http-client";
            });
        }
    }
}
```

Like in the previous case, an instance of `IOptionsSnapshot<WebhookSenderOptions>` is available for injection in the webhook services or in your code.

## Receiving Webhooks

The framework also provides a set of services that can be used to receive webhooks from external systems, and to process them.

To do so, you need to add the `Deveel.Webhooks.Receiver.AspNetCore` library to your project, that must be an _ASP.NET Core_ application.

You can add the library to your project using the `dotnet` command line tool:

``` bash
dotnet add package Deveel.Webhooks.Receiver.AspNetCore
```

or add the following line to your `csproj` file:

``` xml
<PackageReference Include="Deveel.Webhooks.Receiver.AspNetCore" Version="1.1.6" />
```

### Configuring the Receiver

The receiver is configured using the `AddWebhooksReceiver` extension method on the `IServiceCollection` interface.

To run the receiver, you need to add the `UseWebhooksReceiver` middleware to the pipeline of your application, specifying the path where the receiver will be listening for the incoming webhooks.

When the service is configured with a webhook receiver, this will be invoked when a webhook is received, allowing the processing of the incoming webhook.

``` csharp
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

``` csharp
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