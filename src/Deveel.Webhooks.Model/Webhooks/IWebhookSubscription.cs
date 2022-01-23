﻿// Copyright 2022 Deveel
// 
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
// 
//     http://www.apache.org/licenses/LICENSE-2.0
// 
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.

using System;
using System.Collections.Generic;

namespace Deveel.Webhooks {
	/// <summary>
	/// Represents the subscription to the notification
	/// of the occurrence of an event
	/// </summary>
	public interface IWebhookSubscription {
		/// <summary>
		/// Gets the unique identifier of the subscription
		/// </summary>
		string SubscriptionId { get; }

		/// <summary>
		/// Gets a name of the subscription
		/// </summary>
		string Name { get; }

		/// <summary>
		/// Gets a list of types of event to which occurrence
		/// the subscription is activated.
		/// </summary>
		string[] EventTypes { get; }

		/// <summary>
		/// Gets the URL of the receiver of webhooks matching
		/// the subscription
		/// </summary>
		string DestinationUrl { get; }

		/// <summary>
		/// Gets an optional secret used to compute a signature
		/// to secure the webhooks delivered to receivers.
		/// </summary>
		string Secret { get; }

		/// <summary>
		/// Gets the current status of the subscription.
		/// </summary>
		WebhookSubscriptionStatus Status { get; }

		/// <summary>
		/// Gets a maximum number of retries when delivring
		/// a webhook to the receivers (overrides the service 
		/// configurations).
		/// </summary>
		int RetryCount { get; }

		/// <summary>
		/// Gets a set of optional filters to mach the subscription
		/// </summary>
		IEnumerable<IWebhookFilter> Filters { get; }

		/// <summary>
		/// Gets an optional set of headers to be attached to
		/// a HTTP request when attempting the delivery of a
		/// webhook to receivers.
		/// </summary>
		IDictionary<string, string> Headers { get; }

		/// <summary>
		/// Gets an optional set of metadata of the subscription
		/// </summary>
		IDictionary<string, object> Metadata { get; }
	}
}
