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

# Deveel Webhooks Documentation

Here you can find a documentation of the `Deveel Webhooks` framework, to help you getting started with the libraries and functions that compose it and to understand how it works.


## Basic Concepts

To set the domain model of the framework, we need to introduce some basic concepts that are used in the framework.

| Topic                                              | Description                                |
| ---------------------------------------------------- | ------------------------------------------ |
| **[Webhook](concept_webhook.md)**                     | What is it a 'Webhook' and why I need it?  |
| **[Subscriptions](concept_webhook_subscription.md)** | How does a subscription to an event works? |
| **[Senders](concept_webhook_sender.md)**             | What is a sender of webooks?               |
| **[Receivers](concept_webhook_receiver.md)**         | What is a receiver of webooks?             |
| **[Notifications](concept_webhook_notification.md)** | What is a notification of events?         |

## Basic Usage

The following tutorials will guide you through the basic usage of the framework, showing how to use the different components to send, receive and manage webhooks.

| Topic                                                                  | Description                                                           |
| ---------------------------------------------------------------------- | --------------------------------------------------------------------- |
| **[Getting Started](getting_started.md)**                              | Getting started with `Deveel Webhooks`                                |
| **[Sending Webhooks](basic_usage_send.md)**              | Manually sending webhooks (no subscriptions)                          |
| **[Subscription Management](basic_usage_management.md)** | Manage subscriptions to events (no sending)                           |
| **[Notify Webhooks](basic_usage_notify.md)**             | Notify webhooks subscribers (management, transformations and sending) |
| **[Receiving Webhooks](basic_usage_receive.md)**         | Receive webhooks from external sources                                |

## Advanced Usage

A more advanced usage of the framework is possible by implementing custom components that can be used to extend the framework functionalities.

| Topic                                                                              | Description                                                                    |
| ---------------------------------------------------------------------------------- | ------------------------------------------------------------------------------ |
| **[Filtering Subscriptions](advanced_usage_filters.md)**          | Allow subscribers to filter webhooks on dynamic parameters                     |
| **[Custom Data Factories](advanced_usage_custom_datafactory.md)** | Implement a component that transforms event data                               |
| **[Custom Receivers](advanced_usage_custom_receiver.md)**         | Implement parsers for receiving webhooks                                       |
| **[Using Data Layers](advanced_usage_data_layer.md)**             | Use different data layers for the persistence of the webhook information model |

## Receivers

The framework provides a set of libraries that can be used to receive webhooks from external sources.

| Receiver | Description |
| -------- | ----------- |
| **[Facebook](facebook_receiver.md)** | Receive webhooks from Facebook Messenger |
| **[SendGrid](sendgrid_receiver.md)** | Receive webhooks and emails from SendGrid |
| **[Twilio](twilio_receiver.md)** | Receive webhooks and SMS/WhatsApp messages from Twilio |