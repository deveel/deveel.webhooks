<!--
 Copyright 2022 Deveel
 
 Licensed under the Apache License, Version 2.0 (the "License");
 you may not use this file except in compliance with the License.
 You may obtain a copy of the License at
 
     http://www.apache.org/licenses/LICENSE-2.0
 
 Unless required by applicable law or agreed to in writing, software
 distributed under the License is distributed on an "AS IS" BASIS,
 WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
 See the License for the specific language governing permissions and
 limitations under the License.
-->

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

* **Subscription ID**: a unique identifier of the subscription, used to identify the subscription in the system
* **Subscription Name**: a human-readable name of the subscription, used to identify the subscription in the system
* **Event Type**: the type of event the subscription is interested in
* **Event Criteria**: a set of criteria that the event must satisfy in order to be notified to the subscriber
* **Endpoint**: the endpoint where the subscriber will be notified of the event
* **Status**: the status of the subscription, which can be `ACTIVE`, `INACTIVE`, `EXPIRED`, `DELETED`
* **Headers**: a set of headers that will be sent to the subscriber when the event occurs (usefull to identify a context)
* **Secret Key**: a secret key that will be used to sign the notification to the subscriber