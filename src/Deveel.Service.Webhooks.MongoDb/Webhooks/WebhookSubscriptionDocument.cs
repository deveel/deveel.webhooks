using System;
using System.Collections.Generic;

using Deveel.Data;
using Deveel.States;

using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson.Serialization.Options;

namespace Deveel.Webhooks {
	class WebhookSubscriptionDocument : IWebhookSubscription, IMongoDocument, IEntity {
		string IEntity.Id => Id.ToEntityId();

		[BsonId]
		public ObjectId Id { get; set; }

		string IWebhookSubscription.SubscriptionId => Id.ToEntityId();

		public string Name { get; set; }

		public string DestinationUrl { get; set; }

		public string Secret { get; set; }

		public bool IsActive { get; set; }

		public int RetryCount { get; set; }

		public IDictionary<string, string> Headers { get; set; }

		public List<string> EventTypes { get; set; }

		string[] IWebhookSubscription.EventTypes => EventTypes?.ToArray();

		public IList<WebhookFilterField> Filters { get; set; }

		IEnumerable<IWebhookFilter> IWebhookSubscription.Filters => Filters;


		[BsonDictionaryOptions(DictionaryRepresentation.ArrayOfDocuments)]
		public IDictionary<string, object> Metadata { get; set; }
	}
}
