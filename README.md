# Deveel Webhooks

![GitHub release (latest by date)](https://img.shields.io/github/v/release/deveel/deveel.webhooks?display_name=tag&logo=github)
![GitHub Workflow Status](https://img.shields.io/github/actions/workflow/status/deveel/deveel.webhooks/cd.yml?logo=github)
[![Code Coverage](https://codecov.io/gh/deveel/deveel.webhooks/branch/main/graph/badge.svg?token=BKRX2N1IZ1)](https://codecov.io/gh/deveel/deveel.webhooks) 
[![Maintainability](https://api.codeclimate.com/v1/badges/d6af433587d35d4eaee3/maintainability)](https://codeclimate.com/github/deveel/deveel.webhooks/maintainability)
![License](https://img.shields.io/github/license/deveel/deveel.webhooks)

This project provides a set of .NET tools for the management of subscriptions to events, basic transformations and notifications of such event occurrences (_[webhooks](docs/concept_webhook.md)_): in a global design scope, this model enables event-driven architectures, triggering system processes upon the occurrence of expected occurrences from other systems.

Although this integration model is widely adopted by major service providers (like _[SendGrid](https://docs.sendgrid.com/for-developers/tracking-events/getting-started-event-webhook)_, _[Twilio](https://www.twilio.com/docs/usage/webhooks)_, _[GitHub](https://docs.github.com/en/developers/webhooks-and-events/webhooks/about-webhooks)_, _[Slack](https://api.slack.com/messaging/webhooks)_, etc.), there is no formal protocol or authority that would enforce a compliance (like for other cases, such as OpenID, OpenAPI, etc.).

Anyway, a typical implementation consists of the following elements:

* Webhooks are transported through _HTTP POST_ callbacks
* The webhook payload is formatted as a JSON object (or alternatively, in lesser common scenarios, as XML or Form)
* The webhook payload includes properties that describe the type of event and the time-stamp of the occurrence
* An optional signature in the header of the request or a query-string parameter ensures the authenticity of the caller

I tried to express the concepts in more details in [this page](docs/concept_webhook.md) within this repository (without any ambition to be pedagogic).

## The Framework Libraries

The libraries currently provided by the framework are the following:

| Library                                 | Description                                                                                                          | NuGet                                                                  | GitHub (prerelease) |
| ----------------------------------------| ---------------------------------------------------------------------------------------------------------------------|----------------------------------------------------------------------- |---------------------|
| **Deveel.Webhooks**                     | Provides the capabilities to handle webhook subscriptions and notifications                                          | ![Nuget](https://img.shields.io/nuget/v/Deveel.Webhooks?label=NuGet&logo=nuget) | [![GitHub](https://img.shields.io/static/v1?label=GitHub&message=preview&color=yellow&logo=github)](https://github.com/deveel/deveel.webhooks/pkgs/nuget/Deveel.Webhooks) |
| **Deveel.Webhooks.Sender**              | Provides services and functions to send webhooks to remote endpoints                                                 | ![Nuget](https://img.shields.io/nuget/v/Deveel.Webhooks.Sender?label=latest&logo=nuget) | [![GitHub](https://img.shields.io/static/v1?label=GitHub&message=preview&color=yellow&logo=github)](https://github.com/deveel/deveel.webhooks/pkgs/nuget/Deveel.Webhooks.Sender) |
| **Deveel.Webhooks.Service**             | Implements the functions to manage and resolve webhook subscriptions                                                 | ![Nuget](https://img.shields.io/nuget/v/Deveel.Webhooks.Service?label=NuGet&logo=nuget)|  [![GitHub](https://img.shields.io/static/v1?label=NuGet&message=preview&color=yellow&logo=github)](https://github.com/deveel/deveel.webhooks/pkgs/nuget/Deveel.Webhooks.Service) |
| **Deveel.Webhooks.MongoDb**             | An implementation of the webhoom management data layer that is backed by [MongoDB](https://mongodb.com) databases    | ![Nuget](https://img.shields.io/nuget/v/Deveel.Webhooks.MongoDb?label=NuGet&logo=nuget) | [![GitHub](https://img.shields.io/static/v1?label=GitHub&message=preview&color=yellow&logo=github)](https://github.com/deveel/deveel.webhooks/pkgs/nuget/Deveel.Webhooks.MongoDb) |
| **Deveel.Webhooks.DynamicLinq**         | The webhook subscription filtering engine that uses the [Dynamic LINQ](https://dynamic-linq.net/) expressions        | ![Nuget](https://img.shields.io/nuget/v/Deveel.Webhooks.DynamicLinq?label=NuGet&logo=nuget) | [![GitHub](https://img.shields.io/static/v1?label=GitHub&message=preview&color=yellow&logo=github)](https://github.com/deveel/deveel.webhooks/pkgs/nuget/Deveel.Webhooks.DynamicLinq) |
| **Deveel.Webhooks.Receiver.AspNetCore** | An implementation of the webhook receiver that is backed by [ASP.NET Core](https://dotnet.microsoft.com/apps/aspnet) | ![Nuget](https://img.shields.io/nuget/v/Deveel.Webhooks?label=NuGet&logo=nuget) | [![GitHub](https://img.shields.io/static/v1?label=GitHub&message=preview&color=yellow&logo=github)](https://github.com/deveel/deveel.webhooks/pkgs/nuget/Deveel.Webhooks.Receiver.AspNetCore) |

You can obtain the stable versions of these libraries from the [NuGet Official](https://nuget.org) channel.

To get the latest pre-release versions of the packages you can restore from the [Deveel Package Manager](https://github.com/orgs/deveel/packages).


## Motivation

While working on a .NET Core 3.1/.NET 5 *aaS (_as-a-Service_) project that functionally required the capability of users of the service being able to create system-to-system subscriptions and notifications of events through HTTP channel (that is typically named _webhooks_, or _HTTP callbacks_), I started my design with the ambition to use existing solutions, to avoid the bad practice of _reinventing the wheel_, but I ended up frustrated in such ambition:

* [Microsoft's ASP.NET Webhooks](https://github.com/aspnet/WebHooks) project was archived and moved back to the [Microsoft ASP Labs](https://github.com/aspnet/AspLabs/tree/main/src/WebHooks) (that has no visibility on its release), aiming one day to provide compatibility with .NET Core (which eventually evolved, becoming LTS)
* Both Microsoft's projects (the _legacy_ and the _experimental_ ones) are not compatible with the latest .NET stacks (_.NET 5_ / _.NET 6_)
* Microsoft's _experimental_ projects never implemented any capability of handling subscriptions, and eventually removing also the _sender_ capability, focusing exclusively on _receivers_
* Alternative implementations providing similar capabilities are embedded and organic part of larger frameworks (like [ASP.NET Boilerplate](https://github.com/aspnetboilerplate/aspnetboilerplate)), that would have forced me to adopt the the entirety of such frameworks, beyond my design intentions

## Usage Documentation

We would like to help you getting started with this framework and to eventually extend it: please refer to the [Documentation](docs/README.md) section that we have produced for you.

The easiest way to get started is to follow the [Quick Start](docs/QUICKSTART.md) guide, but you can also refer to the [Frequently Asked Questions](docs/FAQS.md) section to get answers to the most common questions.

## A Simple Example

The following example shows how to create a webhook subscription, and how to send a notification to the subscribed endpoint:

```csharp
using Microsoft.AspNetCore.Builder;

using Deveel.Webhooks;

namespace Example {
	public class Program {
		public static void Main(string[] args) {
			var builder = WebApplication.CreateBuilder(args);
			
			// configure your other services ...

			// ... and then configure the webhooks
			builder.Services.AddWebhooks(webhooks => {
				webhooks.AddSubscriptions<MongoWebhookSubscription>(subs => {
					subs.UseMongoDb("mongodb://localhost:27017")
						.UseSubscriptionResolver();
				});

				webhooks.AddNotifier<MyWebhook>(notifier => {
					notifier.UseSender(sender => {
						sender.Configure(options => {
							options.Timeout = TimeSpan.FromSeconds(30);
						});
					});
				});
			});

			var app = builder.Build();

			// configure your other middlewares ...

			// ... and then configure the webhooks
			// to manually send the webhooks ...
			app.MapPost("/webhooks", async (HttpContext context, 
				[FromServices] IWebhookSender<MyWebhook> sender, [FromBody] MyWebhookModel webhook) => {
				var destination = webhook.Destination.ToWebhookDestination();
				var result = await sender.SendAsync(destination webhook, context.HttpContext.RequestAborted);

				// you can log the result of the delivery ...

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

More examples are available in the [Examples](examples/README.md) section.

## Contribute

Contributions to open-source projects, like **Deveel Webhooks**, is generally driven by interest in using the product and services, if they would respect some of the expectations we have to its functions.

The best ways to contribute and improve the quality of this project is by trying it, filing issues, joining in design conversations, and make pull-requests.

Please refer to the [Contributing Guidelines](CONTRIBUTING.md) to receive more details on how you can contribute to this project.

We aim to address most of the questions you might have by providing [documentations](docs/README.md), answering [frequently asked questions](docs/FAQS.md) and following up on issues like bug reports and feature requests.

### Contributors

<a href="https://github.com/deveel/deveel.webhooks/graphs/contributors">
<img src="https://contrib.rocks/image?repo=deveel/deveel.webhooks"/>
</a>

## License Information

This project is released under the [Apache 2 Open-Source Licensing agreement](https://www.apache.org/licenses/LICENSE-2.0).