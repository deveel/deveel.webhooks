# Signing Webhooks

A recommended practice for Webhooks is to sign the payloads of the messages so that the receiver can verify the authenticity and integrity of the message.&#x20;

The framework provides a mechanism to sign the payloads of the messages sent (as _Sender_) and to verify the signatures of the incoming messages (as _Receiver_).

Signature providers are implementations that use the payload of a webhook message and a secret key, to compute a signature to be attached to the webhook, by implementing the `IWebhookSigner` service contract.

By default, when registering a Webhook Sender service, the framework also registers an implementation of a signature provider for the '_HMAC-SHA-256'_ algorithm: it is possible to add custom ones by calling the method `.AddSigner<TSigner>()` of the service builder.

```csharp
using System;

using Microsoft.Extensions.Configuration;

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
            services.AddWebhookSender<MyWebook>(webhooks => {
                // Optional: if not configured by you, the service
                // Add a custom signature provider
                webhooks.AddSigner<MySigner>();
            });
        }
    }
}
```
