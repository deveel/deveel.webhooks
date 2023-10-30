# The WebhookNotifier Service

As described in the [previous chapter](./), when registering the Webhook Notifier service, a number of services are registered in the container, that are used to manage the subscriptions and to deliver the webhooks to the receiving end-point.

An default implementation of the `IWebhookNotifier<TWebhook>` service is also registered by the framework, which is the `WebhookNotifier<TWebhook>` class.

This implementation executes the following steps:

1. Resolution of any webhook subscriptions matching the _event type_ and the _tenant identifier_ (_in case of multi-tenant scenarios_).
2. Transformation of the event into an instance of the webhook, using any service implementing `IWebhookDataFactory<TWebhook>` registered, that can transform or normalize the event information (eg. _resolving a database record from an identifier_), for each of the subscription resolved.
3. Filtering the subscriptions mathing the conditions and criteria configured, against the webhook object constructed in the previous step
4. Attempt to deliver the webhooks to the receiving end-point of the subscription, eventually retrying in case of failures
5. Optional logging of the results of the delivery of the webhooks, when a service implementing `IWebhookDeliveryResultLogger<TWebhook>` is registered in the container

### Service Dependencies

<table data-full-width="true"><thead><tr><th width="421.5">Service</th><th>Description</th></tr></thead><tbody><tr><td><code>IWebhookSubscriptionResolver&#x3C;TWebhook></code></td><td>Resolves the subscriptions to an event. It is generally provided externally, since it's generally tied to the Webhook Subscription service. Anyway, a default implementation of this service is registered, that is a wrapper around any registered <code>IWebhookSubscriptionRepository&#x3C;TSubscription></code> in the container.</td></tr><tr><td><code>IWebhookFactory&#x3C;TWebhook></code></td><td>Transforms the event into a webhook object of the type supported by the service, and that will be then notified.</td></tr><tr><td><code>IWebhookFilterEvaluator&#x3C;TWebhook></code></td><td>The service used to evaluate the filters of the subscriptions. When none is provided at the registration, and webhook subscriptions define any filter, the notification to those subscribers will fail. See <a href="webhook_subscription_filters.md">this chapter</a> for more information</td></tr><tr><td><code>IWebhookDeliveryResultLogger&#x3C;TWebhook></code></td><td>When available in the context of the application, logs the results of the delivery of the webhooks</td></tr></tbody></table>

## Custom Webhook Notifiers

The `WebhookNotifier<TWebhook>` service is registered as `IWebhookNotifier<TWebhook>` in the container, and can be resolved as such: in fact it doesn't have any specific extensions to the original contract.

If you plan to implement your own webhook notifier, you can do so by implementing the `IWebhookNotifier<TWebhook>` interface, and registering it in the container, replacing the default implementation, or by inheriting from the `WebhookNotifier<TWebhook>` class, and overriding the methods you need to customize (that will save you time and preserve from design issues).

You can replace the default implementation of the `IWebhookNotifier<TWebhook>` service by registering your own implementation in the container, as follows:

```csharp
services.AddWebhookNotifier<MyWebhook>()
    .UseNotifier<MyNotifier>();
```

All other default services will still be registered, and you can still use them in your custom implementation, by injecting them in the constructor of your class.
