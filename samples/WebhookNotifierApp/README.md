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
    .UseMongoDb("WebhookSubscriptions");
```

* The `AddWebhooks` method activates the webhook services for the webhooks of type `IdentityWebhook`.
  * The `IdentityWebhook` is a custom webhook type implemented in the scope of this sample, and it is used to send notifications to a webhook endpoint when a user is created.
  * When calling the `AddWebhooks` method without arguments, the webhook services are activated for the default type, that is a `Webhook` oject.

* The `AddNotifier` method configures the default webhook notifier services, activating a default implementation of the sender service.

* The `UseWebhookFactory<UserCreatedWebhookFactory>` method registers the webhook factory that is used to create the webhook for the event `user.created`, that has a `UserCreatedEvent` as payload.
  * More advanced implementations of an application might include a pipeline for the transormation of the incoming event, using the IEventDataFactory instances, to obtain an event that holds a Data property that can be handled by the webhook factory.

* The `UseMongoSubscriptionResolver` method configures the subscription resolver to use a MongoDB database to store the subscriptions - this comes by default with the Deveel.Webhooks.MongoDb package, and uses the `MongoWebhookSubscription` model to store and retrieve the subscriptions.
  - Using MongoDB as the resolver of the subscriptions is a conventience choice for this sample application, but it can be replaced with any other implementation of the `IWebhookSubscriptionResolver` interface.
  - Future implementations might use alternative methods to resolve subscriptions, like a Redis cache, a SQL storage or even a remote service.

* The `AddWebhookSubscriptions` method activates the webhook subscription management services, using the `MongoWebhookSubscription` model to store and retrieve the subscriptions.

* The `UseMongoDb` method configures the MongoDB database as the storage for the subscriptions, using the given connection string.
