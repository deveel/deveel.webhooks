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

using Deveel.Data;

using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson.Serialization.Options;

namespace Deveel.Webhooks {
	public class MongoDbWebhookSubscription : IWebhookSubscription, IMongoDocument {
		string IWebhookSubscription.SubscriptionId => Id.ToEntityId();

		[BsonId]
		public ObjectId Id { get; set; }

		public string Name { get; set; }

		public string DestinationUrl { get; set; }

		public string Secret { get; set; }

		public WebhookSubscriptionStatus Status { get; set; }

		public DateTimeOffset? LastStatusTime { get; set; }

		public string TenantId { get; set; }

		public int RetryCount { get; set; }

		public IDictionary<string, string> Headers { get; set; }

		public List<string> EventTypes { get; set; }

		string[] IWebhookSubscription.EventTypes => EventTypes?.ToArray();

		public IList<MongoDbWebhookFilter> Filters { get; set; }

		IEnumerable<IWebhookFilter> IWebhookSubscription.Filters => Filters;


		[BsonDictionaryOptions(DictionaryRepresentation.ArrayOfDocuments)]
		public IDictionary<string, object> Metadata { get; set; }
	}
}
