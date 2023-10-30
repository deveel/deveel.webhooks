---
layout:
  title:
    visible: true
  description:
    visible: false
  tableOfContents:
    visible: false
  outline:
    visible: true
  pagination:
    visible: true
---

# What is a Sender of Webhooks?

Considering the overall network of communications, where Webhooks are messages exchanged between two applications, a Webhook Sender is an application that sends these messages to another application (or to a multitude of applications).

In real life, you might consider the _Sender_ as the postman delivering a letter or a package to the destination, ensuring the address is correct and exists, that the postbox can hold the shipment, eventually coming back more than once at the address (if the recipient is not at home), accepting the signature, etc.

As much as a postman wouldn't care if the letter or package was in delivery because of an agreement between the recipient and an organization, the same way a Webhook Sender doesn't have to necessarily require the knowledge that an application is expecting the message it is sending (this would be a [Webhook Subscription](webook-subscription.md)).

### The Delivery Destination

A Sender requires the specification of a Destination, to be able to attempt the delivery of Webhooks, which is like a postal address, following the example above.

In most cases, this is simply a URL (_Universal Resource Locator_), that can be reached by an HTTP request, but in some scenarios, this can be enriched with further information instructing the sender on the behavior to have with it, when attempting to deliver a package (for example, we inform the sender to leave the package to our neighbor, if we are not at home, or to try reaching us only on Thursdays, etc.).

Such destination-specific instructions would act as an exception to the regular behavior of the sender, which is described below.

### The Sender Behavior

When sending a Webhook to another party, the application sending messages should ensure that the destination is reachable, the message is well-formatted, its delivery is retried (in case of failures), and the integrity of the content is assured.

This is done by a configured behavior, that typically provides the following elements of configuration

<table><thead><tr><th width="209.5">Attribute</th><th>Description</th></tr></thead><tbody><tr><td><strong>Content Type</strong></td><td>The specification of the format of the contents of the Webhook message (eg. <em>JSON</em>, <em>XML</em>, <em>Form-Encoded</em>)</td></tr><tr><td><strong>Retry Strategy</strong></td><td>The methodology to deliver the message (eg. <em>Circuit Breaker,</em> <em>Exponential Backoff</em>, <em>Timeout</em>, etc.), and the configurations of retries (eg. <em>a maximum number of retries</em>, <em>delays between attempts</em>, etc.)</td></tr><tr><td><strong>Signature Method</strong></td><td>A methodology used to sign the Webhook payloads, so that the the receiver can ensure their integrity</td></tr><tr><td><strong>Context</strong></td><td>The set of contextual information informing the receiver of the origination of the Webhook (eg. <em>the machine name</em>, <em>the application type</em>, etc.)</td></tr></tbody></table>

