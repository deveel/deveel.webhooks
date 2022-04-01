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
