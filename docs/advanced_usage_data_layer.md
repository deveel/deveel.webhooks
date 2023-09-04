<!--
 Copyright 2022-2023 Deveel
 
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

# Webhook Management Data Layers

_**Note**: This part of the framework will sonn go through a major refactoring and the documentation as to be consider provisional_

---

The persistence of information object for long term operations is based on the implementation of a set of contracts of the management domain of the service.

Since the native support of multi-tenancy of the information, a model is in place to create tenant-specific contexts, provided throug a 'store provider' pattern.

## Storage Contracts

The main contracts used to implement this persistence are the following:

| Interface                           | Description                                                                                  |
| ----------------------------------- | -------------------------------------------------------------------------------------------- |
| `IWebhookSubscriptionStore`         | Implements the functions to manage the storage of `IWebhookSubscription` information         |
| `IWebhookSubscriptionStoreProvider` | Provides the means to instantiate a tenant-specific context owning the webhook subscriptions |
| `IWebhookStore`                     | Implements the functions to manage the storage of webhook delivery results                   |
| `IWebhookStoreProvider`             | Creates tenant-specific scopes for the storage of the webhook delivery results               |

## MongoDB Layer

One of the implementations of the storage contract layer provided within the framework is based on the '[MongoDB](https://mongodb.com)' database systems.

To enable this capability in your application, install the `Deveel.Webhook.MongoDb` library.

``` xml
<Project Sdk="Microsoft.NET.Sdk">

  <PropertyGroup>
    <TargetFramework>netcoreapp3.1</TargetFramework>
    ...

  </PropertyGroup>

  <ItemGroup>
    <PackageReference Include="Deveel.Webhooks.MongoDb" Version="1.0.1-alpha1" />
    ...
  </ItemGroup>
</Project>
```

_**Note**: Since the package references the core `Deveel.Webhooks` library, this will also be restored and its functions will be available_

At this point you can configure your application to include the MongoDB layer as this:

``` csharp
namespace Example {
  public class Startup {
	public void ConfigureServices(IServiceCollection services) {
	  services.AddWebhooks()
		.UseMongo(options => {
            options.ConnectionString = "mongodb://localhost:27017";
            options.DatabaseName = "webhooks";
            options.CollectionName = "subscriptions";
            // ...
        });
	}
  }
}

```

## Entity Framework Core Layer

Another implementation of the storage contract layer provided within the framework is based on the '[Entity Framework Core](https://docs.microsoft.com/en-us/ef/core/)' database systems.

To enable this capability in your application, install the `Deveel.Webhook.EntityFrameworkCore` library.

``` xml
<Project Sdk="Microsoft.NET.Sdk">

  <PropertyGroup>
	<TargetFramework>netcoreapp3.1</TargetFramework>
	...

  </PropertyGroup>

  <ItemGroup>
	<PackageReference Include="Deveel.Webhooks.EntityFrameworkCore" Version="2.0.2-beta" />
	...
  </ItemGroup>
</Project>
```

_**Note**: Since the package references the core `Deveel.Webhooks` library, this will also be restored and its functions will be available_

At this point you can configure your application to include the Entity Framework Core layer as this:

``` csharp
namespace Example {
  public class Startup {
	public void ConfigureServices(IServiceCollection services) {
	  services.AddWebhooks()
		.UseEntityFramework(ef => ef.UseDbContext(options => options.UseSqlServer("MyWebhookDb");));
    }
  }
}
```

With the above configuration, the Entity Framework Core layer will use the default implementation of the `WebhookDbContext`, which is based on the `DbContext` class of the Entity Framework Core, configuring the database to use the `SqlServer` provider.

Other database providers can be used by providing the appropriate configuration to the `UseDbContext` method, which accepts a `DbContextOptionsBuilder` instance.