# Webhook Notifier Application

This application is a simple webhook notifier that can be used to send notifications to a webhook endpoint, when a specific event occurs.


The implementation of this sample demonstrates the following practices:

* **Webhook Subscription Management** - The SubscriptionController is wrapped around the WebhookSubscriptionManager that allows to create, update and delete subscriptions, and to retrieve a paged list of all subscriptions.

* **Webhook Notification** - The IdentityEventController intercepts events that are published by an external service and activates the notification to a tenant

* **Webhook Factory** - An instance of the IWebhookFactory is resolving a user from the identifier provided by the event, and creates a webhook that includes the full user's data to be notified

## Configuration

The following configuration code activates the services needed for the webhook notifier application:

```csharp
builder.Services.AddWebhooks<IdentityWebhook>()
    .AddNotifier(notifier => notifier
        .UseWebhookFactory<UserCreatedWebhookFactory>()
	    .UseMongoSubscriptionResolver());

builder.Services.AddWebhookSubscriptions<MongoWebhookSubscription>()
    .UseMongoDb(mongo => mongo.UseMultiTenant());
```

* The `AddWebhooks` method activates the webhook services for the webhooks of type `IdentityWebhook`.
* The `AddNotifier` method configures the default webhook notifier services, activating a default implementation of the sender service.
* The `UseWebhookFactory<UserCreatedWebhookFactory>` method registers the webhook factory that is used to create the webhook for the event `user.created`, that has a `UserCreatedEvent` as payload.
* The `UseMongoSubscriptionResolver` method configures the subscription resolver to use a MongoDB database to store the subscriptions - this comes by default with the Deveel.Webhooks.MongoDb package, and uses the `MongoWebhookSubscription` model to store and retrieve the subscriptions.
* The `AddWebhookSubscriptions` method activates the webhook subscription management services, using the `MongoWebhookSubscription` model to store and retrieve the subscriptions.
* The `UseMongoDb` method configures the MongoDB database as the storage for the subscriptions.
* The `UseMultiTenant` method configures the MongoDB database to be used in a multi-tenant application - this will require that Finbuckle.MultiTenant is configured in the application.

## Notes

* **Multi-tenant Storage** - At the present time, the notifier is limited to the scenarios of webhook notifications to subscribers of a tenant, that requires the configuration of a multi-tenant storage. Future implementations will overcome this limitation, by allowing to store the subscriptions in a single-tenant storage, and to resolve the tenant of the subscriber at the time of the notification.