---
coverY: 0
layout:
  cover:
    visible: true
    size: full
  title:
    visible: true
  description:
    visible: true
  tableOfContents:
    visible: false
  outline:
    visible: false
  pagination:
    visible: true
---

# Deveel Webhooks

Here you can find documentation of the `Deveel Webhooks` framework, to help you get started with the libraries and functions that compose it and to understand how it works.

## Introduction

The framework is composed of a set of libraries that can be used, at different degrees, to implement a system that allows applications to send and receive webhooks.

* **Sending Webhooks** - Using libraries of the framework you can send webhooks to receivers, based on your own logic and rules.
* **Receiving Webhooks** - The framework provides a set of libraries implementing the capabilities for receiving webhooks from senders and reacting to events from external systems (such as Twilio, SendGrid, Facebook, etc.).
* **Notifications** - The framework provides a set of libraries that can be used to manage subscriptions to events and notify subscribing applications of events that occurred in your system.

***

Read more about this framework:

| Topic                                     | Description                                                       |
| ----------------------------------------- | ----------------------------------------------------------------- |
| [**Concepts**](concepts/)                 | A list of basic concepts used in the framework                    |
| [**Getting Started**](getting-started.md) | A quick guide to start using the framework                        |
| [**Sending Webhooks**](send\_webhooks/)   | Sending webhooks messages to receivers                            |
| [**Receiving Webhooks**](receivers/)      | Receiving webhooks from senders                                   |
| [**Notifications**](notifications/)       | Notify subscribing applications of events occurred in your system |
