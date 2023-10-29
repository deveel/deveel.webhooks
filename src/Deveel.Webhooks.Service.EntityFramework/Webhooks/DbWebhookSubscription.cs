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

using System.Diagnostics.CodeAnalysis;

namespace Deveel.Webhooks {
	/// <summary>
	/// Represents the entity that describes a webhook subscription
	/// that is stored in a database.
	/// </summary>
    public class DbWebhookSubscription : IWebhookSubscription {
		/// <summary>
		/// Gets or sets the database identifier of the subscription entity.
		/// </summary>
        public string? Id { get; set;}

        string? IWebhookSubscription.SubscriptionId => Id;

		/// <summary>
		/// Gets or sets the identifier of the tenant that owns 
		/// the subscription
		/// </summary>
        public string? TenantId { get; set; }

		/// <inheritdoc/>
        public string Name { get; set; }

		/// <inheritdoc/>
        public string DestinationUrl { get; set; }

		/// <inheritdoc/>
        public string? Secret { get; set; }

		/// <inheritdoc/>
        public string? Format { get; set; }

		/// <inheritdoc/>
        public WebhookSubscriptionStatus Status { get; set; }

		/// <inheritdoc/>
        public int? RetryCount { get; set; }

		/// <inheritdoc/>
        public DateTimeOffset? CreatedAt { get; set; }

		/// <inheritdoc/>
        public DateTimeOffset? UpdatedAt { get; set; }

		[ExcludeFromCodeCoverage]
        IEnumerable<string> IWebhookSubscription.EventTypes =>
            Events?.Select(x => x.EventType) ?? Array.Empty<string>();

		/// <summary>
		/// Gets or sets the list of events that are subscribed.
		/// </summary>
        public virtual List<DbWebhookSubscriptionEvent> Events { get; set; }

        IEnumerable<IWebhookFilter>? IWebhookSubscription.Filters => Filters.AsReadOnly();

		/// <summary>
		/// Gets or sets the list of filters that are applied to the subscription.
		/// </summary>
        public List<DbWebhookFilter> Filters { get; set; }

        IDictionary<string, string>? IWebhookSubscription.Headers =>
            Headers?.ToDictionary(x => x.Key, x => x.Value);

		[ExcludeFromCodeCoverage]
        IDictionary<string, object>? IWebhookSubscription.Properties =>
            Properties?.ToDictionary(x => x.Key, x => DbWebhookValueConvert.Convert(x.Value, x.ValueType)!);

		/// <summary>
		/// Gets or sets the list of headers that are applied to the subscription.
		/// </summary>
        public virtual List<DbWebhookSubscriptionHeader> Headers { get; set; }

		/// <summary>
		/// Gets or sets the list of properties of the subscription.
		/// </summary>
        public virtual List<DbWebhookSubscriptionProperty> Properties { get; set; }
    }
}
