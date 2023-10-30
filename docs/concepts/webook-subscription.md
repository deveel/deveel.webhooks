# What is a Webhook Subscription?

It is easy to define the concept of subscription to the notification of a certain type of events, and we have several examples of this in our daily life:

* We can subscribe to a newspaper to receive it every day,
* We can be opt-in to be notified when a new product is available in a shop by a phone call or an email
* We can subscribe to a newsletter to be notified when a new article is published on a blog

In all these cases, we are interested in being notified when something happens, and we are not interested in the details of the event itself: some of the elements of the event are relevant to us, and some of these elements are relevant for the publisher to know how and when to reach us.

For the other party that has to notify us on the type of events we are interested in, it is important to know how to reach us, and it is important to know what we are interested in.

In the case of a newspaper, the publisher needs to know our address, and the type of newspaper we want to receive. In the case of a shop, the publisher needs to know our phone number or email address, and the type of products we are interested in. In the case of a blog, the publisher needs to know our email address, and the type of articles we are interested in.

## Webhook Subscription

Information systems work partially in the same way: as owners of a system, we can subscribe to a certain type of events ocurring in an external system, and we can specify the endpoint, reachable from a HTTP channel, where the system will be notified when these events happen.

The publisher system is then requested to keep a record of all these information, so that when an event occurs, it can notify all the subscribers that are interested in that event, given a set of criteria.

In fact, a system can assume both roles of publisher and subscriber, where it can be notified of events from an external system, and then notify other systems of events occurring in its own scope (or in some cases just routing the events received).

Subscriptions to events are typically indicating used by publisher to control the behavior of the notification process.

Although no formal specification for this type of objects was agreed and standardized, a typical pattern across the providers of services is to define the following elements:



| Attribute                | Description                                                                                                |
| ------------------------ | ---------------------------------------------------------------------------------------------------------- |
| **Subscription ID**      | A unique identifier of the subscription, used to identify the subscription in the system                   |
| **Subscription Name**    | A human-readable name of the subscription, used to identify the subscription in the system                 |
| **Event Type**           | The type of event the subscription is interested in                                                        |
| **Event Criteria**       | A set of criteria that the event must satisfy in order to be notified to the subscriber                    |
| **Destination Endpoint** | The endpoint where the subscriber will be notified of the event                                            |
| **Status**               | The status of the subscription, which can be `ACTIVE`, `INACTIVE`, `EXPIRED`, `DELETED`                    |
| **Headers**              | A set of headers that will be sent to the subscriber when the event occurs (usefull to identify a context) |
| **Secret Key**           | A secret key that will be used to sign the notification to the subscriber                                  |
