# Webhook Management Data Layers

_**Note**: This part of the framework will sonn go through a major refactoring and the documentation as to be consider provisional_

---

The persistence of information object for long term operations is based on the implementation of a set of contracts of the management domain of the service.

Since the native support of multi-tenancy of the information, a model is in place to create tenant-specific contexts, provided throug a 'store provider' pattern.

## Storage Contracts

The main contracts used to implement this persistence are the following:

| Interface                             | Description                                                                                  |
| ------------------------------------- | -------------------------------------------------------------------------------------------- |
| `IWebhookSubscriptionStore`           | Implements the functions to manage the storage of `IWebhookSubscription` information         |
| `IWebhookSubscriptionStoreProvider`   | Provides the means to instantiate a tenant-specific context owning the webhook subscriptions |
| `IWebhookDeliveryResultStore`         | Implements the functions to manage the storage of webhook delivery results                   |
| `IWebhookDeliveryResultStoreProvider` | Creates tenant-specific scopes for the storage of the webhook delivery results               |


## Subscription Resolution

Although the resolution of the webhook subscriptions is not directly related to the storage of the information, the framework provides a contract to implement the resolution of the subscriptions based on the storage.

In fact, the `IWebhookSubscriptionResolver` interface is used by the framework to resolve the subscriptions to a specific event, and in the default implementation, it uses the `IWebhookSubscriptionStore` to retrieve the information.

## Delivery Logging

Some advanced scenarios of usage may require to log the delivery results of the webhook notifications, to provide a way to track the delivery status of the notifications.

Even if the logging mechanism doesn't require a specific database (it might be logging on CSV files, JSON, etc.), the framework provides the contract `IWebhookDeliveryResultStore` to implement the storage of the delivery results.

## Storage Implementations

The _Deveel Webhooks_ framework provides the following implementations of the storage:

| Implementation                                   | Description                                                                                | Subscription Store | Delivery Logging     | Multi-Tenant         |
| ------------------------------------------------ | ------------------------------------------------------------------------------------------ | :----------------: | :------------------: | :------------------: |
| [`MongoDB Storage`](./advanced_usage_mongodb.md) | Implements the storage of the webhook subscriptions and delivery results using MongoDB     | :white_check_mark: | :white_check_mark:   | :white_check_mark:   |
| [`Entity Framework`](./advanced_usage_ef.md)     | An implementation that stores subcriptions and delivery results using the Entity Framework | :white_check_mark: | :white_check_mark:   | :x:                  |

Refere to the specific documentation of each implementation for more details.