# Webhook Notifications

The notification process of a webhook introduces elements of automation, putting together the _[subscription management](webhook_subscription_management.md)_ and the _[sending of webhooks](../sending-webhooks/README.md)_ processes, and an optional step of _[data transformation](custom_datafactory.md)_ (to resolve event information in a full formed object to be transferred).

The _Deveel Webhook_ framework implements these functions through an instance of the `IWebhookNotifier<TWebhook>` service, that uses a `IWebhookSubscriptionResolver` and the `IWebhookSender<TWebhook>` instance configured.
