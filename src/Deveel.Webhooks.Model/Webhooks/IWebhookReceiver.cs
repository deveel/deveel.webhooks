// Copyright 2022-2025 Antonello Provenzano
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

namespace Deveel.Webhooks {
	/// <summary>
	/// Describes the receiver of a webhook notification
	/// </summary>
	public interface IWebhookReceiver {
		/// <summary>
		/// If the notification was performed to a specific subscription,
		/// this returns the identifier of the subscription.
		/// </summary>
		string? SubscriptionId { get; }

		/// <summary>
		/// If the notification was performed to a specific subscription,
		/// this returns the name of the subscription.
		/// </summary>
		string? SubscriptionName { get; }

		/// <summary>
		/// Gets the URL where the notification was sent.
		/// </summary>
		string DestinationUrl { get; }

		/// <summary>
		/// Gets the list of headers sent with the notification.
		/// </summary>
		IEnumerable<KeyValuePair<string, string>> Headers { get; }

		/// <summary>
		/// Gets a code that identifies the format of the request
		/// body content. (either <c>json</c>, <c>xml</c> or <c>form</c>).
		/// </summary>
		string BodyFormat { get; }
	}
}
