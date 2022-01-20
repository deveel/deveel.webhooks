// Copyright 2022 Deveel
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
	/// Notifies the occurrence of an event
	/// and trasports related data
	/// </summary>
	public interface IWebhook {
		/// <summary>
		/// Gets an unique identifier of the event
		/// </summary>
		string Id { get; }

		/// <summary>
		/// Gets the name of the webhook (eg. the name of the
		/// subscription)
		/// </summary>
		string Name { get; }

		/// <summary>
		/// Gets the format of the content of the webhook
		/// </summary>
		string Format { get; }

		/// <summary>
		/// If the webhook was originated by a subscription,
		/// this gets its unique identifier.
		/// </summary>
		string SubscriptionId { get; }

		/// <summary>
		/// Gets the URL of the receiver.
		/// </summary>
		string DestinationUrl { get; }

		/// <summary>
		/// Gets an optional secret used to compute the signature 
		/// of the webhook
		/// </summary>
		string Secret { get; }

		/// <summary>
		/// Gets a set of additional headers to be included
		/// in the webhook during the delivery.
		/// </summary>
		IDictionary<string, string> Headers { get; }

		/// <summary>
		/// Gets the exact time of the event occurrence.
		/// </summary>
		DateTimeOffset TimeStamp { get; }

		/// <summary>
		/// Gets the type of the event
		/// </summary>
		string EventType { get; }

		/// <summary>
		/// Gets the data carried by the webhook to the receiver
		/// </summary>
		object Data { get; }
	}
}
