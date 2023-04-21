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
using System.Collections.Generic;

using Deveel.Data;

using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson.Serialization.Options;

namespace Deveel.Webhooks {
	public class MongoDbWebhook : IWebhook {
		string IWebhook.Id => WebhookId;

		public string WebhookId { get; set; }

		public string Name { get; set; }

		public string Format { get; set; }

		public string SubscriptionId { get; set; }

		public string DestinationUrl { get; set; }

		public string Secret { get; set; }

		[BsonDictionaryOptions(DictionaryRepresentation.ArrayOfDocuments)]
		public IDictionary<string, string> Headers { get; set; }

		public DateTimeOffset TimeStamp { get; set; }

		public string EventType { get; set; }

		// TODO: convert this to a dynamic object?
		object IWebhook.Data => Data;

		public BsonDocument Data { get; set; }
	}
}
