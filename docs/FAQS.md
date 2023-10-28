# Frequently Asked Questions

## Q: Why have you developed another Webhook management library

A: We didn't have the ambition to develop this project, but rather to use some already available, anyway given the conditions we were in, we could not find any fitting alternative:

* [**Microsoft's ASP.NET Webhook Framework**](https://github.com/aspnet/WebHooks), before being retired, supported only the .NET 4.6 framework
* [**Microsot's ASP.NET Core Webhook Framework**](https://github.com/aspnet/AspLabs/tree/main/src/WebHooks) was _demoted_ to an experimental project (within the scope of [AspNetLabs](https://github.com/aspnet/AspLabs) space), and anyway did not provide any capability for the management of subscriptions, or logging results of deliveries
* [**ASP.NET Boilerplate (by Volosoft)**](https://github.com/aspnetboilerplate/aspnetboilerplate) provides functionalities for the management and sending of webhooks that are embedded into a more extended framework, that we didn't want to use in its entirety.

## Q: Which .NET versions are supported by Deveel Webhooks?

A: Since the version 2.0.1, the framework is built on top of .NET 6.0, and therefore it can be used in any .NET implementation that supports this version of the standard.

## Q: Do you have any commercial plans for this framework?

A: No. Not at the moment.

The origin of this project was to support a commercial service, that is currently under development, and we wanted to provide the community with the outcomes of our experiences and finding in this specific area.

## Q: Is your aim to replace Microsoft's Webhook Framework?

A: As pointed out in the answer provided above (on the motivations of this project), currently Microsoft provides no stable alternatives to handle webhook subscription management, but just an experimental framework to implement receivers of webhooks from major service providers.

## Q: Do you provide any other data layers than MongoDB?

A: Not at the moment, but any contribution is welcome... :)

The data model of subscriptions and webhooks is not complex and should not be a challenge to contribute with alternatives (please refer to the [contributing guidelines](../CONTRIBUTING.md)).

## Q: Does Deveel Webhooks support webhook subscriptions?

A: Yes. The server part of the framework provides a mechanism to manage webhook subscriptions, that can be used to register a webhook endpoint to receive webhooks from your applications (as provider).

## Q: Does Deveel Webhooks support webhook formats other than JSON?

A: Yes. Since version _2.0.1_, the framework supports JSON and XML formats for sending webhooks, while for the receiving part it support JSON, XML and Forms (_application/x-www-form-urlencoded_ Content-Type) formats, that is dependent on the receiver implementation.

## Q: Does Deveel Webhooks support webhook authentication?

A: Yes. Since version _2.0.1_, the framework supports the validation of the signature of the webhook payload, using the HMAC-SHA256 and HMAC-SHA1 algorithms, and the secret key provided by the subscription.

## Q: Does Deveel Webhooks support webhook encryption?

A: Not at the moment.

## Q: Does Deveel Webhooks support webhook retry policies?

A: Yes. The framework supports the definition of a retry policy for the delivery of webhooks, that is applied when the delivery of a webhook fails. The policy can be defined at the level of the subscription, or at the level of the webhook itself.

## Q: Does Deveel Webhooks support webhook delivery scheduling?

A: No. It is not in the scope of the framework to provide a scheduler for the delivery of webhooks. The framework provides a mechanism to trigger the delivery of webhooks, but it is up to the application to implement a scheduler that triggers the delivery of webhooks.

## Q: Does Deveel Webhooks support webhook delivery logging?

A: Yes. The framework provides a mechanism to log the results of the delivery of webhooks, that can be used to persist the results of the delivery of webhooks. The framework provides a default implementation of the logging mechanism that uses the data layer to persist the results of the delivery of webhooks.

## Q: Does Deveel Webhooks support webhook delivery throttling?

A: No. At the moment the framework does not provide any mechanism to throttle the delivery of webhooks, but this is something that we are considering to implement in the future.

## Q: Does Deveel Webhooks support webhook delivery batching?

A: Not at the moment, but we have included in the issues as an idea to implement in the future.

## Q: Does Deveel Webhooks support webhook delivery deduplication?

A: No. It is a good idea to explore for future implementations.

## Q: Which webhook providers are supported by Deveel Webhooks?

A: At the moment the framework supports the following external webhook providers:

* [**Facebook**](receivers/facebook\_receiver.md)
* [**SendGrid**](receivers/sendgrid\_receiver.md)
* [**Twilio**](receivers/twilio\_receiver.md)

Follow the issues of this project to see which providers are planned to be supported in the future.

The framework also provides a generic receiver that can be used to implement a webhook receiver for any other provider, including your own applications.
