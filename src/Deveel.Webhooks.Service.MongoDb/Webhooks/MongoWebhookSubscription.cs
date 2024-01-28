// Copyright 2022-2024 Antonello Provenzano
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
using System.Diagnostics.CodeAnalysis;

using Deveel.Data;

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
	public class MongoWebhookSubscription : IWebhookSubscription, IHaveTimeStamp {
		[ExcludeFromCodeCoverage]
		string? IWebhookSubscription.SubscriptionId => Id.Equals(ObjectId.Empty) ? null : Id.ToString();

		/// <summary>
		/// Gets or sets the identifier of the subscription entity.
		/// </summary>
		[BsonId, Key]
		public ObjectId Id { get; set; }

		/// <inheritdoc/>
		[Column("name")]
		public string Name { get; set; }

		/// <inheritdoc/>
		[Column("destination_url")]
        public string DestinationUrl { get; set; }

		/// <inheritdoc/>
		[Column("secret")]
        public string? Secret { get; set; }

		/// <inheritdoc/>
		[Column("status")]
        public WebhookSubscriptionStatus Status { get; set; }

		/// <inheritdoc/>
		[Column("tenant_id")]
        public string TenantId { get; set; }

        /// <inheritdoc/>
        public int? RetryCount { get; set; }

		/// <inheritdoc/>
		[Column("headers")]
		[BsonDictionaryOptions(DictionaryRepresentation.Document)]
        public IDictionary<string, string> Headers { get; set; }

		/// <inheritdoc/>
		[Column("format")]
        public string Format { get; set; }

        /// <inheritdoc/>
        [Index("by_event_type", IndexSortOrder.Descending)]
		[Column("event_types")]
		public List<string> EventTypes { get; set; }

		[ExcludeFromCodeCoverage]
		IEnumerable<string> IWebhookSubscription.EventTypes => EventTypes?.ToArray() ?? Array.Empty<string>();

		/// <inheritdoc/>
		[Column("filters")]
        public IList<MongoWebhookFilter> Filters { get; set; }

		[ExcludeFromCodeCoverage]
		IEnumerable<IWebhookFilter> IWebhookSubscription.Filters => Filters;


		/// <inheritdoc/>
		[Column("properties")]
        [BsonDictionaryOptions(DictionaryRepresentation.ArrayOfDocuments)]
		public IDictionary<string, object> Properties { get; set; }

		DateTimeOffset? IHaveTimeStamp.CreatedAtUtc {
			get => CreatedAt ?? DateTimeOffset.UtcNow;
			set => CreatedAt = value;
		}

		/// <inheritdoc/>
		[Column("created_at")]
        public DateTimeOffset? CreatedAt { get; set; }

		DateTimeOffset? IHaveTimeStamp.UpdatedAtUtc {
			get => UpdatedAt ?? DateTimeOffset.UtcNow;
			set => UpdatedAt = value;
		}

		/// <inheritdoc/>
		[Column("updated_at")]
        public DateTimeOffset? UpdatedAt { get; set; }
	}
}
