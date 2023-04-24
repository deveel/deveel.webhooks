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

using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson.Serialization.Options;

using MongoFramework;
using MongoFramework.Attributes;

namespace Deveel.Webhooks {
	/// <summary>
	/// An entity that represents a subscription to a webhook
	/// and that is stored in a MongoDB storage.
	/// </summary>
	[Table(MongoDbWebhookStorageConstants.SubscriptionCollectionName)]
	public class MongoWebhookSubscription : IWebhookSubscription {
		string? IWebhookSubscription.SubscriptionId => Id.Equals(ObjectId.Empty) ? null : Id.ToString();

		/// <summary>
		/// Gets or sets the identifier of the subscription entity.
		/// </summary>
		[BsonId, Key]
		public ObjectId Id { get; set; }

		/// <inheritdoc/>
		public string Name { get; set; }

        /// <inheritdoc/>
        public string DestinationUrl { get; set; }

        /// <inheritdoc/>
        public string Secret { get; set; }

        /// <inheritdoc/>
        public WebhookSubscriptionStatus Status { get; set; }

		/// <summary>
		/// Gets or sets the time when the last status of the subscription
		/// was set.
		/// </summary>
		public DateTimeOffset? LastStatusTime { get; set; }

        /// <inheritdoc/>
        public string TenantId { get; set; }

        /// <inheritdoc/>
        public int? RetryCount { get; set; }

        /// <inheritdoc/>
        public IDictionary<string, string> Headers { get; set; }

        /// <inheritdoc/>
        public string Format { get; set; }

        /// <inheritdoc/>
        [Index("by_event_type", IndexSortOrder.Descending)]
		public List<string> EventTypes { get; set; }

		IEnumerable<string> IWebhookSubscription.EventTypes => EventTypes?.ToArray() ?? Array.Empty<string>();

        /// <inheritdoc/>
        public IList<MongoWebhookFilter> Filters { get; set; }

		IEnumerable<IWebhookFilter> IWebhookSubscription.Filters => Filters;


        /// <inheritdoc/>
        [BsonDictionaryOptions(DictionaryRepresentation.ArrayOfDocuments)]
		public IDictionary<string, object> Properties { get; set; }

        /// <inheritdoc/>
        public DateTimeOffset? CreatedAt { get; set; }

        /// <inheritdoc/>
        public DateTimeOffset? UpdatedAt { get; set; }
	}
}
