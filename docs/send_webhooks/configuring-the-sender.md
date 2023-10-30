# Configuring the Sender

### Configuring the Delivery Behavior

The framework provides a default behavior for the delivery of webhooks to the registered recipients, but you can configure the behavior of the service to suit your needs.

The library `Deveel.Webhooks` depends from the `Deveel.Webhooks.Sender` library, which provides the needed functions to send webhooks to receivers: you will find the abstractions and helpers to configure the behavior of the webhook delivery to recipient systems, controlling aspects of the process like _payload formatting_, _retries on failures_, _signatures_.

You have several options to configure the service, and therefore you are free to chose the methodology that suits you best.

### The WebhookSenderOptions

The `WebhookSenderOptions` class provides a set of properties that can be used to configure the behavior of the webhook sender.

The configurations are evolving with the versions of the framework, and the following ones apply to the current version (`1.1.6`) of the library.

```csharp
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

```csharp
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
            services.AddWebhookSender<MyWebhook>("Webhooks:Sender");
        }
    }
}
```

After this, an instance of `IOptions<WebhookDeliveryOptions>` is available for injection in the webhook services or in your code.

Given the design of the service, it will also be possible to access a webhook-specific instance of `IOptions<WebhookDeliveryOptions>` for a given webhook type, using the `IOptionsSnapshot<TOptions>` service, using the type name of the webhook as the key (eg. `IOptionsSnapshot<WebhookSenderOptions>.Get("MyWebhook")`).

### Manual Configuration

If you prefer to configure the service manually, you can use the `AddWebhooks` overload that accepts an instance of `WebhookSenderOptions` as parameter.

```csharp
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

            services.AddWebhookSender<MyWebhook>(options => {
              options.HttpClientName = "my-http-client";
            });
        }
    }
}
```

Like in the previous case, an instance of `IOptionsSnapshot<WebhookSenderOptions>` is available for injection in the webhook services or in your code.
