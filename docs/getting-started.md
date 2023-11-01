# Getting Started

The overall design of this framework is open and extensible (implementing the traditional [Open-Closed Principle](https://en.wikipedia.org/wiki/Open%E2%80%93closed\_principle)), which means base contracts can be extended, composed, or replaced.

It is possible to use its components as they are provided or use the base contracts to extend single functions, while still using the rest of the provisioning.

### Sending and Receiving

The framework provides three major capabilities to the applications using its libraries

<table><thead><tr><th width="209.5">Capability</th><th>Description</th></tr></thead><tbody><tr><td><a href="send_webhooks/"><strong>Send Webhooks</strong></a></td><td>Send a Webhook message to a receiver, enforcing formatting, integrity and retry rules</td></tr><tr><td><a href="notifications/"><strong>Notify Webhooks</strong></a></td><td>Communicate the occurrence of an event in a system to an external application that is listening for those events </td></tr><tr><td><a href="receivers/"><strong>Receive Webhooks</strong></a></td><td>Accepts and processes a notification from an external system, to trigger any related process</td></tr></tbody></table>

The two sending capabilities (_send_ and _notify_) are disconnected from the receiving capability, since they represent two different parts of the communication channel (the _Sender_ and the _Receiver_): as such the architecture of the framework is designed so that they don't depend on each other's.

