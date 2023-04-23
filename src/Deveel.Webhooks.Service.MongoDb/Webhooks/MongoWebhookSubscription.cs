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
	[Table(MongoDbWebhookStorageConstants.SubscriptionCollectionName)]
	public class MongoWebhookSubscription : IWebhookSubscription {
		string? IWebhookSubscription.SubscriptionId => Id.Equals(ObjectId.Empty) ? null : Id.ToString();

		[BsonId, Key]
		public ObjectId Id { get; set; }

		public string Name { get; set; }

		public string DestinationUrl { get; set; }

		public string Secret { get; set; }

		public WebhookSubscriptionStatus Status { get; set; }

		public DateTimeOffset? LastStatusTime { get; set; }

		public string TenantId { get; set; }

		public int? RetryCount { get; set; }

		public IDictionary<string, string> Headers { get; set; }

		public string Format { get; set; }

		[Index("by_event_type", IndexSortOrder.Descending)]
		public List<string> EventTypes { get; set; }

		IEnumerable<string> IWebhookSubscription.EventTypes => EventTypes?.ToArray() ?? Array.Empty<string>();

		public IList<MongoWebhookFilter> Filters { get; set; }

		IEnumerable<IWebhookFilter> IWebhookSubscription.Filters => Filters;


		[BsonDictionaryOptions(DictionaryRepresentation.ArrayOfDocuments)]
		public IDictionary<string, object> Metadata { get; set; }

		public DateTimeOffset? CreatedAt { get; set; }

		public DateTimeOffset? UpdatedAt { get; set; }
	}
}
