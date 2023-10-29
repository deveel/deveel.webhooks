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

using System.Diagnostics.CodeAnalysis;

// Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
#pragma warning disable CS8618

namespace Deveel.Webhooks {
	/// <summary>
	/// Represents a webhook receiver that is stored in a database
	/// </summary>
    public class DbWebhookReceiver : IWebhookReceiver {
		/// <summary>
		/// Gets or sets the database identifier of the 
		/// receiver entity
		/// </summary>
        public int? Id { get; set; }

		/// <summary>
		/// Gets or sets the database identifier of the
		/// subscription entity that owns the receiver.
		/// </summary>
        public string? SubscriptionId { get; set; }

		/// <summary>
		/// Gets or sets the database entity that describes
		/// the subscription that owns the receiver.
		/// </summary>
        public virtual DbWebhookSubscription? Subscription { get; set; }

		/// <inheritdoc/>
        public string? SubscriptionName { get; set; }

		/// <inheritdoc/>
        public string DestinationUrl { get; set; }

		[ExcludeFromCodeCoverage]
        IEnumerable<KeyValuePair<string, string>> IWebhookReceiver.Headers
            => Headers?.ToDictionary(x => x.Key, y => y.Value) ?? new Dictionary<string, string>();

		/// <summary>
		/// Gets or sets the list of headers that are sent
		/// to the receiver.
		/// </summary>
		/// <seealso cref="IWebhookReceiver.Headers"/>
        public virtual List<DbWebhookReceiverHeader> Headers { get; set; }

		/// <inheritdoc/>
        public string BodyFormat { get; set; }
    }
}
