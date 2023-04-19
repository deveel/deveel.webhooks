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

## Basic Concepts

| Concept                                              | Description                                |
| ---------------------------------------------------- | ------------------------------------------ |
| **[Webhook](concept_webhook.md)**                     | What is it a 'Webhook' and why I need it?  |
| **[Subscriptions](concept_webhook_subscription.md)** | How does a subscription to an event works? |
| **[Receivers](concept_webhook_receiver.md)**         | What is a receiver of webooks?             |

## Tutorials

| Topic                                                                  | Description                                                           |
| ---------------------------------------------------------------------- | --------------------------------------------------------------------- |
| **[Getting Started](getting_started.md)**                              | Getting started with `Deveel Webhooks`                                |
| **[Basic Usage - Sending Webhooks](basic_usage_send.md)**              | Manually sending webhooks (no subscriptions)                          |
| **[Basic Usage - Subscription Management](basic_usage_management.md)** | Manage subscriptions to events (no sending)                           |
| **[Basic Usage - Notify Webhooks](basic_usage_notify.md)**             | Notify webhooks subscribers (management, transformations and sending) |
| **[Basic Usage - Receiving Webhooks](basic_usage_receive.md)**         | Receive webhooks from external sources                                |

## Extending

| Topic                                                                              | Description                                                                    |
| ---------------------------------------------------------------------------------- | ------------------------------------------------------------------------------ |
| **[Advanced Usage - Filtering Subscriptions](advanced_usage_filters.md)**          | Allow subscribers to filter webhooks on dynamic parameters                     |
| **[Advanced Usage - Custom Data Factories](advanced_usage_custom_datafactory.md)** | Implement a component that transforms event data                               |
| **[Advanced Usage - Custom Receivers](advanced_usage_custom_receiver.md)**         | Implement parsers for receiving webhooks                                       |
| **[Advanced Usage - Using Data Layers](advanced_usage_data_layer.md)**             | Use different data layers for the persistence of the webhook information model |
