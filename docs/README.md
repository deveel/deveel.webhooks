# Deveel Webhooks Documentation

Here you can find a documentation of the `Deveel Webhooks` framework, to help you getting started with the libraries and functions that compose it and to understand how it works.

## Introduction

The framework is composed by a set of libraries that can be used, at different degrees, to implement a system that allows applications to send and receive webhooks.

* **Sending Webhooks** - Using libraries of the framework you can send webhooks to receivers, based on your own logic and rules.
* **Receiving Webhooks** - The framework provides a set libraries implementing the capabilities for receiving webhooks from senders, reacting to events from external systems (such as Twilio, SendGrid, Facebook, etc.).
* **Notifications** - The framework provides a set of libraries that can be used to manage subscriptions to events, notify subscribing applications of events occurred in your system.

---

Read more about this framework:

| Topic  |  Description  |
| ------ | ------------- |
| **[Concepts](concepts/README.md)** | A list of basic concepts used in the framework |
| **[Getting Started](getting_started.md)** | A quick guide to start using the framework |
| **[Sending Webhooks](send_webhooks.md)** | Sending webhooks messages to receivers |
| **[Receiving Webhooks](receivers/README.md)** | Receiving webhooks from senders |
| **[Notifications](notifications/README.md)** | Notify subscribing applications of events occurred in your system |
