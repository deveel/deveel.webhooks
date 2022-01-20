<!--
 Copyright 2022 Deveel
 
 Licensed under the Apache License, Version 2.0 (the "License");
 you may not use this file except in compliance with the License.
 You may obtain a copy of the License at
 
     http://www.apache.org/licenses/LICENSE-2.0
 
 Unless required by applicable law or agreed to in writing, software
 distributed under the License is distributed on an "AS IS" BASIS,
 WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
 See the License for the specific language governing permissions and
 limitations under the License.
-->

# Producing Custom Data Factories

In automated scenarios of notification, when an event triggers a process of construction of the webhooks and their delivery to subscribers, there could be a need to _transform_ the original data carried by those triggering events.

In fact, sometimes events don't carry all information that have to be transmitted to the webhook receivers, for several reasons (privacy, design, external context, etc.), and this requires an additional intervention for the integration of those information.

_Note_: This is not a mandatory passage in the notification process, and can be skipped if the data carried by the event represents the information to be transferred to the receivers.

## Transformation of Data

Data transformations are specific to your use cases and they can be specific to a given event condition (eg. _only a specific type of event with a given value in its 'data' component triggers the transformation_).

To implement this logic in the application, you must first create a new class that inherits from the `IWebhookDataFactory` contract.

```csharp
using System;

using Deveel.Webhooks.

namespace Example {
    public class MyWebhookDataFactory : IWebhookDataFactory {
        // assess if the event given can be handled
        public bool Handles(EventInfo eventInfo) {
            // implement your verification logic here ...
            return true;
        }

        // this constructs a new object that will be used as
        // part of the payload of the webhook
        public async Task<object> CreateDataAsync(EventInfo eventInfo, CancellationToken cancellationToken) {
            // here you can use external services to combine and form the result
            // object, according to your logic...
            return null;
        }
    }
}

```

To enable the implemented by your data factory, you can register it during the configuration of the application.

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
            services.AddWebhooks(webhooks => {
                // This call registers the data factory as a
                // scoped service by default, but other overloads
                // allow controlling the lifetime
                webhooks.AddDataFactory<MyWebhookDataFactory>();
            });
        }
    }
}

```

The [webhook notifier component](basic_usage_notification.md) will resolve the first data factory supporting the event triggering the notification, and trasform the data.

**Note**: Currently only the service implements one single transformation per event (_or none_). If multiple data factories matching the event are registered, only the last one will be used. (_Probably there is a room for improvement in this area, implementing a pipelined transformation..._)

## Default Webhook Fields

Additionally to the data of the event, eventually transformed (as explained above), by design convention a webhook consists also of few additional elements, to reflect its nature of _event_ (see the [webhook concept](concept_webhook.md))

| Field      | Description                                      |
| ---------- | ------------------------------------------------ |
| Event ID   | The unique identifier of the original event      |
| Event Type | The type of the original event that was fired    |
| Time-Stamp | The exact time when the original event was fired |

It is possible to control the inclusion of these fields during the process of formation of the webhook, through configuration of the application.

``` csharp
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
            services.AddWebhooks(webhooks => {
                // This configuration instructs the sender to
                // include the Event ID, Event Type and Time-Stamp
                // in the payload of the webhook
                webhooks.ConfigureDelivery(delivery =>
                    delivery.IncludeAllFields());
            });
        }
    }
}

```
