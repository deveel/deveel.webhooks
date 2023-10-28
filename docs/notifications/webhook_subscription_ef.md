The [Entity Framework](https://learn.microsoft.com/en-us/ef/) is an advanced [ORM](https://en.wikipedia.org/wiki/Object%E2%80%93relational_mapping) for the .NET enviroment that allows the abstraction of the domain model of the application from the database-specific commands.

The _Deveel Webhooks_ framework provides an implementation of the storage layer that uses the Entity Framework to store the data, primarily in relational in databases: it is not intended to be used with a specific SQL database vendor.

The implementation of the storage layer is provided by the `Deveel.Webhooks.EntityFramework` package.

## Installation

The package is available on [NuGet](https://www.nuget.org/packages/Deveel.Webhooks.EntityFramework) and can be installed using the following command from the NuGet Package Manager Console:

```powershell
Install-Package Deveel.Webhooks.EntityFramework
```

Alternatively, you can use the `dotnet` CLI:

```bash
dotnet add package Deveel.Webhooks.EntityFramework
```

## Configuration

Once the package is installed, you can configure the storage layer in the `Startup` class of your application, by using the `AddEntityFrameworkStorage` extension method:

```csharp
public void ConfigureServices(IServiceCollection services) {
    // ...
    services.AddWebhookSubscriptions(webhook => {
        webhook.UseEntityFramework(ef => {
            ef.UseContext(options => {
				options.UseSqlServer(Configuration.GetConnectionString("Webhooks"));
			});
        });
    });
    // ...
}
```

The `UseEntityFramework` method accepts a callback that allows to configure the storage layer: the callback is provided with an instance of `EntityFrameworkStorageBuilder` that allows to configure the storage layer.

One of the methods of the builder is `UseContext` that allows to configure the database context to use: the callback is provided with an instance of `DbContextOptionsBuilder` that allows to configure the database context: you can provide your own implementation of the database context, or use one of the pre-defined implementations provided by the framework.

### Pre-defined Database Context

The framework provides a pre-defined database context `WebhookDbContext` that can be used to configure the storage layer: this applies the default entity configurations.