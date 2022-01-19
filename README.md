# Deveel Webhooks

[![GitHub Actions](https://github.com/deveel/deveel.webhooks/actions/workflows/cd.yml/badge.svg)](https://github.com/deveel/deveel.webhooks/actions/workflows/cd.yml) [![Code Coverage](https://codecov.io/gh/deveel/deveel.webhooks/branch/main/graph/badge.svg?token=BKRX2N1IZ1)](https://codecov.io/gh/deveel/deveel.webhooks)

This project provides a set of .NET tools for the management of subscriptions to events, basic transformations and notifications of such event occurrences (_webhooks_): in a global design scope, this model enables event-driven architectures, triggering system processes upon the occurrence of expected occurrences from other systems.

Although this integration model is widely adopted by major service providers (like _[SendGrid](https://docs.sendgrid.com/for-developers/tracking-events/getting-started-event-webhook)_, _[Twilio](https://www.twilio.com/docs/usage/webhooks)_, _[GitHub](https://docs.github.com/en/developers/webhooks-and-events/webhooks/about-webhooks)_, _[Slack](https://api.slack.com/messaging/webhooks)_, etc.), there is no formal protocol or authority that would enforce a compliance (like for other cases, such as OpenID, OpenAPI, etc.).

Anyway, a typical implementation consists of the following elements:

* Webhooks are transported through _HTTP POST_ callbacks
* The webhook payload is represented as a JSON object
* The webhook payload includes properties that describe the type of event and the time-stamp of the occurrence
* An optional signature in the header of the request or a query-string parameter ensures the authenticity of the caller

## Motivation

While working on a .NET Core 3.1/.NET 5 *aaS (_as-a-Service_) project that functionally required the capability of users of the service being able to create system-to-system subscriptions and notifications (typically named _webhooks_), I started my design with the ambition to use existing solutions, to avoid the bad practice of _reinventing the wheel_, but I ended up frustrated in such ambition:

* [Microsoft's ASP.NET Webhooks](https://github.com/aspnet/WebHooks) project was archived and moved back to the [Microsoft ASP Labs](https://github.com/aspnet/AspLabs), not providing any visibility on its status, and not being compatible with .NET Core (and even less with the stack .NET 5/6)
* Even in its _experimental_ status, the **Microsoft ASP.NET Webhooks** framework removed the capability of handling subscriptions, being mainly focused on _receivers_
* Alternative implementations were included and organic part of frameworks (like [ASP.NET Boilerplate](https://github.com/aspnetboilerplate/aspnetboilerplate)), that would have forced me to adopt the the entirety of such frameworks, beyond my design intentions

## Usage Documentation

You can refer to the [Documentation](docs/README.md) provided to getting you started using the framework, and eventually extending it.

## Contribute

Contributions to open-source projects, like **Deveel Webhooks**, is generally driven by interest in using the product and services, if they would respect some of the expectations we have to its functions.

The best ways to contribute and improve the quality of this project is by trying it, filing issues, joining in design conversations, and make pull-requests.

Please refer to the [Contributing Guidelines](CONTRIBUTING.md) to receive more details on how you can contribute to this project.

We aim to address most of the questions you might have by providing [documentations](docs/README.md), answering [frequently asked questions](docs/FAQS.md) and following up on issues like bug reports and feature requests.

## License Information

This project is released under the [Apache 2 Open-Source Licensing agreement](https://www.apache.org/licenses/LICENSE-2.0).
