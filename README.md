# Deveel Webhooks

![GitHub release (latest by date)](https://img.shields.io/github/v/release/deveel/deveel.webhooks?display_name=tag&logo=github)
![GitHub Workflow Status](https://img.shields.io/github/actions/workflow/status/deveel/deveel.webhooks/cd.yml?logo=github)
[![Code Coverage](https://codecov.io/gh/deveel/deveel.webhooks/branch/main/graph/badge.svg?token=BKRX2N1IZ1)](https://codecov.io/gh/deveel/deveel.webhooks) 
[![Maintainability](https://api.codeclimate.com/v1/badges/d6af433587d35d4eaee3/maintainability)](https://codeclimate.com/github/deveel/deveel.webhooks/maintainability)
![License](https://img.shields.io/github/license/deveel/deveel.webhooks)

This project provides a set of .NET tools for the management of subscriptions to events, basic transformations and notifications of such event occurrences (_[webhooks](docs/concepts/webhook.md)_): in a global design scope, this model enables event-driven architectures, triggering system processes upon the occurrence of expected occurrences from other systems.

Although this integration model is widely adopted by major service providers (like _[SendGrid](https://docs.sendgrid.com/for-developers/tracking-events/getting-started-event-webhook)_, _[Twilio](https://www.twilio.com/docs/usage/webhooks)_, _[GitHub](https://docs.github.com/en/developers/webhooks-and-events/webhooks/about-webhooks)_, _[Slack](https://api.slack.com/messaging/webhooks)_, etc.), there is no formal protocol or authority that would enforce a compliance (like for other cases, such as OpenID, OpenAPI, etc.).

Anyway, a typical implementation consists of the following elements:

* Webhooks are transported through _HTTP POST_ callbacks
* The webhook payload is formatted as a JSON object (or alternatively, in lesser common scenarios, as XML or Form)
* The webhook payload includes properties that describe the type of event and the time-stamp of the occurrence
* An optional signature in the header of the request or a query-string parameter ensures the authenticity of the caller

I tried to express the concepts in more detail in [this page](docs/concept_webhook.md) within this repository (without any ambition to be pedagogic).

## The Framework Libraries

The libraries currently provided by the framework are the following:

| Library |  NuGet  | GitHub (prerelease) |
| --- |--- | --- |
| **Deveel.Webhooks** | [![Nuget](https://img.shields.io/nuget/v/Deveel.Webhooks?label=NuGet&logo=nuget)](https://www.nuget.org/packages/Deveel.Webhooks) | [![GitHub](https://img.shields.io/static/v1?label=GitHub&message=preview&color=yellow&logo=github)](https://github.com/deveel/deveel.webhooks/pkgs/nuget/Deveel.Webhooks) |
| **Deveel.Webhooks.Sender**  | [![Nuget](https://img.shields.io/nuget/v/Deveel.Webhooks.Sender?label=latest&logo=nuget)](https://www.nuget.org/packages/Deveel.Webhooks.Sender) | [![GitHub](https://img.shields.io/static/v1?label=GitHub&message=preview&color=yellow&logo=github)](https://github.com/deveel/deveel.webhooks/pkgs/nuget/Deveel.Webhooks.Sender) |
| **Deveel.Webhooks.Service**  | [![Nuget](https://img.shields.io/nuget/v/Deveel.Webhooks.Service?label=NuGet&logo=nuget)](https://www.nuget.org/packages/Deveel.Webhooks.Service) |  [![GitHub](https://img.shields.io/static/v1?label=NuGet&message=preview&color=yellow&logo=github)](https://github.com/deveel/deveel.webhooks/pkgs/nuget/Deveel.Webhooks.Service) |
| **Deveel.Webhooks.MongoDb**  | [![Nuget](https://img.shields.io/nuget/v/Deveel.Webhooks.MongoDb?label=NuGet&logo=nuget)](https://www.nuget.org/packages/Deveel.Webhooks.MongoDb) | [![GitHub](https://img.shields.io/static/v1?label=GitHub&message=preview&color=yellow&logo=github)](https://github.com/deveel/deveel.webhooks/pkgs/nuget/Deveel.Webhooks.MongoDb) |
| **Deveel.Webhooks.EntityFramework** | [![Nuget](https://img.shields.io/nuget/v/Deveel.Webhooks.EntityFramework?label=NuGet&logo=nuget)](https://www.nuget.org/packages/Deveel.Webhooks.EntityFramework) | [![GitHub](https://img.shields.io/static/v1?label=GitHub&message=preview&color=yellow&logo=github)](https://github.com/deveel/deveel.webhooks/pkgs/nuget/Deveel.Webhooks.EntityFramework) |
| **Deveel.Webhooks.DynamicLinq**  | [![Nuget](https://img.shields.io/nuget/v/Deveel.Webhooks.DynamicLinq?label=NuGet&logo=nuget)](https://www.nuget.org/packages/Deveel.Webhooks.DynamicLinq) | [![GitHub](https://img.shields.io/static/v1?label=GitHub&message=preview&color=yellow&logo=github)](https://github.com/deveel/deveel.webhooks/pkgs/nuget/Deveel.Webhooks.DynamicLinq) |
| **Deveel.Webhooks.Receiver.AspNetCore** | [![Nuget](https://img.shields.io/nuget/v/Deveel.Webhooks.Receiver.AspNetCore?label=NuGet&logo=nuget)](https://www.nuget.org/packages/Deveel.Webhooks.Receiver.AspNetCore) | [![GitHub](https://img.shields.io/static/v1?label=GitHub&message=preview&color=yellow&logo=github)](https://github.com/deveel/deveel.webhooks/pkgs/nuget/Deveel.Webhooks.Receiver.AspNetCore) |

The following libraries extend the framework with receivers for specific providers:

| Library | NuGet  | GitHub (prerelease) |
| --- | --- |---|
| **Deveel.Webhooks.Receiver.Twilio**  | [![Nuget](https://img.shields.io/nuget/v/Deveel.Webhooks.Receiver.Twilio?label=NuGet&logo=nuget)](https://www.nuget.org/packages/Deveel.Webhooks.Receiver.Twilio) | [![GitHub](https://img.shields.io/static/v1?label=GitHub&message=preview&color=yellow&logo=github)](https://github.com/deveel/deveel.webhooks/pkgs/nuget/Deveel.Webhooks.Receiver.Twilio) |
| **Deveel.Webhooks.Receiver.SendGrid**  | [![Nuget](https://img.shields.io/nuget/v/Deveel.Webhooks.Receiver.SendGrid?label=NuGet&logo=nuget)](https://www.nuget.org/packages/Deveel.Webhooks.Receiver.SendGrid) | [![GitHub](https://img.shields.io/static/v1?label=GitHub&message=preview&color=yellow&logo=github)](https://github.com/deveel/deveel.webhooks/pkgs/nuget/Deveel.Webhooks.Receiver.SendGrid) |
| **Deveel.Webhooks.Receiver.Facebook**  | [![Nuget](https://img.shields.io/nuget/v/Deveel.Webhooks.Receiver.Facebook?label=NuGet&logo=nuget)](https://www.nuget.org/packages/Deveel.Webhooks.Receiver.Facebook) | [![GitHub](https://img.shields.io/static/v1?label=GitHub&message=preview&color=yellow&logo=github)](https://github.com/deveel/deveel.webhooks/pkgs/nuget/Deveel.Webhooks.Receiver.Facebook) |

You can obtain the stable versions of these libraries from the [NuGet Official](https://nuget.org) channel.

To get the latest pre-release versions of the packages you can restore from the [Deveel Package Manager](https://github.com/orgs/deveel/packages).

## Documentation

We would like to help you get started with this framework and to eventually extend it: please refer to the **[Documentation](docs/README.md)** section, or to the **[Official Website](https://webhooks.deveel.org/)** that we have produced for you.

The easiest way to get started is to follow the **[Getting Started](docs/getting-started.md)** guide, but you can also refer to the **[Frequently Asked Questions](docs/FAQS.md)** section to get answers to the most common questions.

## Motivation

While working on a .NET Core 3.1/.NET 5 _PaaS_ (_Platform-as-a-Service_) project that functionally required the capability of users of the service being able to create system-to-system subscriptions and notifications of events through HTTP channel (that is typically named _webhooks_, or _HTTP callbacks_), I started my design with the ambition to use existing solutions, to avoid the bad practice of _reinventing the wheel_, but I ended up frustrated in such ambition:

* [Microsoft's ASP.NET Webhooks](https://github.com/aspnet/WebHooks) project was archived and moved back to the [Microsoft ASP Labs](https://github.com/aspnet/AspLabs/tree/main/src/WebHooks) (that has no visibility on its release), aiming one day to provide compatibility with .NET Core (which eventually evolved, becoming LTS)
* Both Microsoft's projects (the _legacy_ and the _experimental_ ones) are not compatible with the latest .NET stacks (_.NET 5_ / _.NET 6_)
* Microsoft's _experimental_ projects never implemented any capability of handling subscriptions, and eventually removed the _sender_ capability, focusing exclusively on _receivers_
* Alternative implementations providing similar capabilities are embedded and organic parts of larger frameworks (like [ASP.NET Boilerplate](https://github.com/aspnetboilerplate/aspnetboilerplate)), that would have forced me to adopt the the entirety of such frameworks, beyond my design intentions

## Simple Usage Example

The **[documentation of the framework](docs/README.md)** will provide you with more details on the requirements, configurations, usage and extensibility of the framework.

Anyway, to help you get started with the framework, please consider the following examples that show how to create a simple webhook management service, that handle subscriptions and notifications, and a client receiver.

### Subscriptions and Notifications

As a provider of service, this library provides functions to handle the two main aspects of the webhook pattern:

* **Subscriptions**: the capability of a client to subscribe to a specific event, providing an endpoint to be notified
* **Notifications**: the capability of a server to send notifications to the subscribed endpoints

The following example shows how to create a webhook subscription, and how to send a notification to the subscriber endpoints:

```csharp
using Microsoft.AspNetCore.Builder;

using Deveel.Webhooks;

namespace Example {
    public class Program {
        public static void Main(string[] args) {
            var builder = WebApplication.CreateBuilder(args);
            
            // ...

            builder.Services.AddWebhookSubscriptions<MongoWebhookSubscription>(subs => { 
                subs.UseMongoDb("mongodb://localhost:27017")
                    .UseSubscriptionResolver();
            });
				
            builder.Services.AddWebhookNotifier<MyWebhook>(notifier => {
                notifier.UseSender(sender => {
                   sender.Configure(options => {
                        options.Timeout = TimeSpan.FromSeconds(30);
                   });
               });
            });

            var app = builder.Build();

            // ...

            // ... and notify the receivers manually ...
            app.MapPost("/webhooks", async (HttpContext context, 
                [FromServices] IWebhookSender<MyWebhook> sender, [FromBody] MyWebhookModel webhook) => {
                var destination = webhook.Destination.ToWebhookDestination();
                var result = await sender.SendAsync(destination, webhook, context.HttpContext.RequestAborted);

                // ...

                return Results.Ok();
            });

            // ... or notify the webhooks automatically from subscriptions
            app.MapPost("/webhooks/notify", async (HttpContext context, 
                [FromServices] IWebhookNotifier<MyWebhook> notifier, [FromBody] MyEventModel eventModel) => {
                var eventInfo = eventModel.AsEventInfo();
                var result = await notifier.NotifyAsync(eventInfo, context.HttpContext.RequestAborted);

                // you can log the result of the notification to all receivers ...
                return Results.Ok();
            });

            app.Run();
        }
    }
}
```

## Receivers

The framework also provides a set of built-in receivers that can be used to handle the incoming notifications from the subscribed endpoints in your application.

The following example shows how to create a receiver for a webhook that is backed by a [Facebook Messenger](https://facebook.com) message:

```csharp
namespace Example {
    public class Program {
        public static void Main(string[] args) {
            var builder = WebApplication.CreateBuilder(args);
            
            // ...

            builder.Services.AddFacebookReceiver()
                .AddHandler<MyFacebookWebhookHandler>();

            var app = builder.Build();

            // ...

            // ... you can handle all the incoming webhooks at "/webhooks/facebook"
            // invoking all the handlers registered in the service collection ...
            app.MapFacebookWebhook("/webhooks/facebook");

            // ... or you can handle the incoming webhooks manually ...

            app.MapFacebookWebhook("/webhooks/facebook2", async (FacebookWebhook webhook, IService service, CancellationToken ct) => {
                // ...
                await service.DoSomethingAsync(webhook, ct);
            });

            app.Run();
        }
    }
}
```


## Contribute

Contributions to open-source projects, like **Deveel Webhooks**, is generally driven by interest in using the product and services, if they would respect some of the expectations we have for its functions.

The best ways to contribute and improve the quality of this project are by trying it, filing issues, joining in design conversations, and making pull-requests.

Please refer to the [Contributing Guidelines](CONTRIBUTING.md) to receive more details on how you can contribute to this project.

We aim to address most of the questions you might have by providing [documentations](docs/README.md), answering [frequently asked questions](docs/FAQS.md) and following up on issues like bug reports and feature requests.

### Contributors

<a href="https://github.com/deveel/deveel.webhooks/graphs/contributors">
<img src="https://contrib.rocks/image?repo=deveel/deveel.webhooks"/>
</a>

## License Information

This project is released under the [Apache 2 Open-Source Licensing agreement](https://www.apache.org/licenses/LICENSE-2.0).
