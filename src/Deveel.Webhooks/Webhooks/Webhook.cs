// Copyright 2022-2023 Deveel
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

// Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
#pragma warning disable CS8618

using System;
using System.Text.Json.Serialization;
using System.Xml.Serialization;

using Deveel.Webhooks.Json;

namespace Deveel.Webhooks {
	/// <summary>
	/// A default implementation of <see cref="IWebhook"/> that can be used
	/// to transport the data of a notification, provided for convenience.
	/// </summary>
	/// <remarks>
	/// <para>
	/// The sender and notifier services are not constrained to a specific
	/// type of webhook, and this class is used to create the default webhook,
	/// implementing the <see cref="IWebhook"/> constract.
	/// </para>
	/// <para>
	/// By default this class is configured with attributes from
	/// the <c>System.Text.Json</c> library and the <c>System.Xml</c> namespace, so that 
	/// the serialization process recognizes them and uses them to serialize the data:
	/// if you need to use other serializers you might experience inconsistent results.
	/// </para>
	/// </remarks>
	public class Webhook : IWebhook {
		/// <inheritdoc/>
		[JsonPropertyName("event_type")]
		[XmlAttribute("type")]
		public string EventType { get; set; }

		/// <inheritdoc/>
		[JsonPropertyName("event_id")]
		[XmlAttribute("id")]
		public string Id { get; set; }

		/// <inheritdoc/>
		[JsonPropertyName("timestamp")]
		[JsonConverter(typeof(UnixTimeMillisJsonConverter))]
		[XmlAttribute("timeStamp")]
		public DateTimeOffset TimeStamp { get; set; }

		/// <inheritdoc/>
		[JsonPropertyName("data")]
		[XmlElement(ElementName = "data")]
		public object Data { get; set; }

		/// <inheritdoc/>
		[JsonPropertyName("subscription_id")]
		[XmlAttribute("subscriptionId")]
		public string? SubscriptionId { get; set; }

		/// <summary>
		/// Gets the name of webhook (the name of the subscription).
		/// </summary>
		[JsonPropertyName("name")]
		[XmlAttribute("name")]
		public string? Name { get; set; }
	}
}
