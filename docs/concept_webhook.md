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

# What is a Webhook?

## The Event Model

If we consider the overall model of _processes_, where an activity is performed, and eventually preceding other activities, until a final result, we can define the _events_ as the triggers of such activities (eg. _something has occurred, and therefore we must do some activities_).
Equally we can consider that some activities _produce_ the information that an event has occurred, to make aware interested actors to react (eg. _while performing this activity, this has happened: feel free to act consequentially_).

Academic studies have analyzed this concept more than a century ago, and some formal representations of this have been designed and adopted by the Information Technology industry (eg. _BPMN_, _Workflow_, etc.), in order to rationalize and optimize processes within the information systems (although, not all activities are performed by systems: some _actors_ in such processeses are humans).

For example, one of the most common of these formal models, the _Business Process Management Notation (BPMN)_, defines several types of these events, expressed as _Triggers_ (those that start a process, like _a message_ or _a timer_), _Intermediate Events_ (those produced during an activity of a process) or _Terminal Events_ (those that cause the end of the process).

If you think about it, this happens in your daily life as well:

- _Today I **woke up** and I **brushed my teeth**_ - You are informing the listener that two events occurred today: you (the actor) woke up and you brushed your teeth
- _While I was crossing the street, a car **horned** at me_ - You are informing the listener that _a car_ (the actor) horned at you

## Event-Driven Design

The development and operational model of systems has greatly benefit by the adoption of designs where those systems were loosely coupled with other systems, implementing asynchronous operations based on the occurrence of events produced elsewhere: this approach, that today seems a _given_ for many developers, has optimized the overall performances and maintainability of services, which are able to isolate their functions, and instead of _pulling_ the event at regular intervals (eg. _querying an external service every X minutes to see if anything has happened_), can be resiliently _listening_ and _reacting_ only when needed.

This design has also produced extensibility opportunities, since it made it possible for a system to be more easily integrated (once the events and their information is known), since the dependency from the components is not direct and the load is moved to the transportation medium, rather than being provided by the service itself (eg. _the I/O to the databases is reduced or removed, since the event carrying all needed information has been produced_).

## The Webhook Model

Moving out of the conceptual model, the initial problem faced to export _events_ out of the boundaries of the systems of a service provider to external listening applications consisted in the format and protocol of the transportation.

The best solution found along the way (and at today still the most used) to notify such events is through _HTTP callbacks_: a _POST_ request through the _HTTP protocol_ to a publicly exposed end-point belonging to an application _accepting_ such notifications.

This methodology has been commonly denominated by the industry as _**Webhook**_.

At today, there is still no agreed common protocol to define the format and contents of a _Webhook_, although some elements represent a pattern across the various implementations available:

- The callback request is performed using the _POST_ verb of the _HTTP_ protocol
- The payload (the content) of the request is formatted as a _JSON_ or _XML_ text (although some implementations use the _www-form-urlencoded_ format)
- The request _optionally_ includes an header or a query string element that expresses a _signature_ that is interpretable by the subscribing application (to be sure of the genuinity of the information)

