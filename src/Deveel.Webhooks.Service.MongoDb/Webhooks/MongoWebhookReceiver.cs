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


namespace Deveel.Webhooks {
    /// <summary>
    /// An object that represents the information about a receiver
    /// of a webhook that is stored in a MongoDB storage.
    /// </summary>
    public class MongoWebhookReceiver : IWebhookReceiver {
		/// <summary>
		/// Gets or sets the URL endpoint where the webhook
		/// was delivered.
		/// </summary>
		public string DestinationUrl { get; set; }

		IEnumerable<KeyValuePair<string, string>> IWebhookReceiver.Headers => Headers;

		/// <summary>
		/// Gets or sets the list of headers that were sent
		/// alongside the webhook, if any.
		/// </summary>
		public IDictionary<string, string> Headers { get; set; }

		/// <summary>
		/// Gets or sets the format of the body of the webhook
		/// (either 'json' or 'xml')
		/// </summary>
		public string BodyFormat { get; set; }

		/// <summary>
		/// Gets or sets the unique identifier of the subscription
		/// that triggered the delivery of the webhook (if the webhook
		/// was actually notified from a subscription).
		/// </summary>
		public string? SubscriptionId { get; set; }

		/// <summary>
		/// Gets or sets the name of the subscription that triggered
		/// the delivery of the webhook (if the webhook was actually
		/// notified from a subscription).
		/// </summary>
		public string? SubscriptionName { get; set; }
	}
}
