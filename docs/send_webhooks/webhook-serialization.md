# Webhook Serialization

The most common format for the payloads of Webhooks is the JSON format, but it is possible to use also the XML format, as long as the receiver is able to understand it. The framework provides a mechanism to serialize the payloads of the messages sent to the receivers.

The framework provides a set of default implementation of the serializers



<table data-full-width="false"><thead><tr><th width="246">Type</th><th width="168">Library</th><th>Description</th></tr></thead><tbody><tr><td>SystemTextWebhookJsonSerializer</td><td>Deveel.Webhooks</td><td>An implementation that uses the System.Text.Json serialization functions (recommended)</td></tr><tr><td>NewtonsoftWebhookJsonSerializer</td><td>Deveel.Webhooks.NewtonsoftJson</td><td>Implements the webhook serialization by using the Newtonsoft.Json serializers</td></tr><tr><td>SystemWebhookXmlSerializer</td><td>Deveel.Webhooks</td><td>Serializes webhooks to XML using the System.Xml native functions</td></tr></tbody></table>

When initializing the sender, a default serializer for each format is added to the service, but it is possible to add custom ones through the call to `.UseJsonSerializer<TSerializer>()` or `.UseXmlSerializer<TSerializer>()` of the service builder.

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
                webhooks.UseJsonSerializer<MyJsonSerializer>();
                webhooks.UseXmlSerializer<MyXmlSerializer>();
            });
        }
    }
}
```
