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

using System;
using System.Text.Json.Serialization;

namespace Deveel.Webhooks {
	/// <summary>
	/// A default implementation of <see cref="IWebhook"/> that can be used
	/// to transport the data of a notification.
	/// </summary>
	public class Webhook : IWebhook {
		/// <inheritdoc/>
		[JsonPropertyName("event_type")]
		public string EventType { get; set; }

		/// <inheritdoc/>
		[JsonPropertyName("event_id")]
		public string Id { get; set; }

		/// <inheritdoc/>
		[JsonPropertyName("timestamp")]
		[JsonConverter(typeof(UnixTimeSecondsJsonConverter))]
		public DateTimeOffset TimeStamp { get; set; }

		/// <inheritdoc/>
		[JsonPropertyName("data")]
		public object Data { get; set; }

		/// <inheritdoc/>
		[JsonPropertyName("subscription_id")]
		public string SubscriptionId { get; set; }
	}
}
