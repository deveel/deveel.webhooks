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

# Deveel Webhooks Frequently Asked Questions

## Q: Why have you developed another Webhook management library

A: We didn't have the ambition to develop this project, but rather to use some already available, anyway given the conditions we were in, we could not find any fitting alternative:

* **[Microsoft's ASP.NET Webhook Framework](https://github.com/aspnet/WebHooks)**, before being retired, supported only the .NET 4.6 framework
* **[Microsot's ASP.NET Core Webhook Framework](https://github.com/aspnet/AspLabs/tree/main/src/WebHooks)** was _demoted_ to an experimental project (within the scope of [AspNetLabs](https://github.com/aspnet/AspLabs) space), and anyway did not provide any capability for the management of subscriptions, or logging results of deliveries
* **[ASP.NET Boilerplate (by Volosoft)](https://github.com/aspnetboilerplate/aspnetboilerplate)** provides functionalities for the management and sending of webhooks that are embedded into a more extended framework, that we didn't want to use in its entirety.

## Q: Do you have any commercial plans for this framework?

A: No. Not at the moment.

The origin of this project was to support a commercial service, that is currently under development, and we wanted to provide the community with the outcomes of our experiences and finding in this specific area.

## Q: Is your aim to replace Microsoft's Webhook Framework?

A: As pointed out in the answer provided above (on the motivations of this project), currently Microsoft provides no stable alternatives to handle webhook subscription management, but just an experimental framework to implement receivers of webhooks from major service providers.

## Q: Do you provide any other data layers than MongoDB?

A: Not at the moment, but any contribution is welcome... :)

The data model of subscriptions and webhooks is not complex and should not be a challenge to contribute with alternatives (please refer to the [contributing guidelines](/CONTRIBUTING.md)).

## Q: Will Deveel Webhooks support webhook formats other than JSON?

A: We are planning to introduce support XML and potentially Forms (_www-form-urlencoded_) formats, but this would require some major changes in the serialization and signature processes, and it's low in priority.
