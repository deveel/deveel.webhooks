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

# Getting Started

The overall design of this little framework is open and extensible (implementing the traditional [Open-Closed Principle](https://en.wikipedia.org/wiki/Open%E2%80%93closed_principle)), that means base contracts can be extended, composed or replaced.

It is possible to use its components as they are provided, or use the base contracts to extend single functions, while still using the rest of the provisioning.

## Intall the Required Libraries

The overall set of libraries are available through [NuGet](https://nuget.org), and can be installed and restored easily once configured in your projects.

At the moment (_January 2022_) they are developed as `.NET Standard 2.1` and thus compatible with all the profiles of the .NET framework greater or equal than the `.NET Core 3.1`.

The core library of the framework is `Deveel.Webhooks` and can be installed as follow

``` xml
<Project Sdk="Microsoft.NET.Sdk">

  <PropertyGroup>
    <TargetFramework>netcoreapp3.1</TargetFramework>
    ...

  </PropertyGroup>

  <ItemGroup>
    <PackageReference Include="Deveel.Webhooks" Version="1.0.1-alpha1" />
    ...
  </ItemGroup>
</Project>
```

This provides all the functions that are needed to send webhooks to a given destination and activate the notification process (although this one would require external instrumentations, for resolving subscriptions and other advanced functions).

## The Framework Libraries

The libraries currently provided by the framework are the following:

| Library                             | Description                                                                                                       |
| ----------------------------------- | ----------------------------------------------------------------------------------------------------------------- |
| **Deveel.Webhooks.Model**           | The foundation library that defines the webhooks information model                                                |
| **Deveel.Webhooks**                 | Provides the foundation contracts of the webhook service and basic implementations for the sending functions      |
| **Deveel.Webhooks.Service**         | Implements the functions to manage and resolve webhook subscriptions                                              |
| **Deveel.Webhooks.Service.MongoDb** | An implementation of the webhoom management data layer that is backed by [MongoDB](https://mongodb.com) databases |
| **Deveel.Webhooks.DynamicLinq**     | The webhook subscription filtering engine that uses the [Dynamic LINQ](https://dynamic-linq.net/) expressions     |

You can obtain the stable versions of these libraries from the [NuGet Official](https://nuget.org) channel.

For the _nighly builds_ and previews you can restore from the [Deveel Package Manager](https://github.com/orgs/deveel/packages).

## Configuring the Delivery Behavior

Included in the core library of the framework (`Deveel.Webhooks`) you will find the abstractions and helpers to configure the behavior of the webhook delivery to recipient systems: this controls aspects of the process like _payload formatting_, _retries on failures_, _signatures_.

You have several options to configure the service, and therefore you are free to chose the methodology that suits you best.

### The WebhookDeliveryOptions

``` csharp

var options = new WebhookDeliveryOptions {
    // The maximum number of delivery attempts
    MaxAttemptCount = 2,

    // The maximum time before failing a single delivery
    TimeOut = TimeSpan.FromSeconds(2),

    // Instructs the service to sign the webhooks
    // using the secrets of the subscriptions
    SignWebhooks = true,

    // If the webhooks are signed, where to place
    // the signature ('Header' or 'QueryString') in the
    // delivery HTTP Request
    SignatureLocation = WebhookSignatureLocation.Header,

    // The algorithm to be used to sign the webhooks
    // (among the registered ones)
    SignatureAlgorithm = "HMAC-SHA-256",

    // If the signature is to be placed in the headers
    // of a request, this sets the name of the header
    // carrying that signature
    SignatureHeaderName = "X-WEBHOOK-SIGNATURE",

    // If the signature is to be placed in the query-string
    // of a request, this sets the key of the entry
    // carrying that signature
    SignatureQueryStringKey = "webhook-signature"
};

```

### IConfiguration Pattern

If you keep your configurations in an `appsetting.json` file, environment variables, secrets, implementing a typical pattern of the _ASP.NET_ applications, you can invoke the overloads provided by `Deveel.Webhook` that access the available instances of `IConfiguration`.

Using the `IConfiguration` object injected at Startup

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
            // The configurations are specified in the 'Webhooks:Delivery'
            // section of the configuration instance
            services.AddWebhooks(webhooks => 
                webhooks.ConfigureDelivery(Configuration, "Webhooks:Delivery"));
        }
    }
}
```

After this, an instance of `IOptions<WebhookDeliveryOptions>` is available for injection in the webhook services or in your code.

## Dependency Pattern

If you want to let the services to access the available instance of the `IConfiguration` after the build of the service provider, you can use another overload.

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
            // The configurations are specified in the 'Webhooks:Delivery'
            // section of the configuration instance
            services.AddWebhooks(webhooks => 
                webhooks.ConfigureDelivery("Webhooks:Delivery"));
        }
    }
}
```

Like in the previous case, an instance of `IOptions<WebhookDeliveryOptions>` is available for injection in the webhook services or in your code.
