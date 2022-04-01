using System;
using System.Collections.Generic;

using Deveel.Data;

using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace Deveel.Webhooks {
	public class MongoDbWebhookDeliveryResult : IWebhookDeliveryResult, IMongoDocument, IMultiTenantDocument {
		[BsonId]
		public ObjectId Id { get; set; }

		IWebhook IWebhookDeliveryResult.Webhook => Webhook;

		public virtual MongoDbWebhook Webhook { get; set; }

		IEnumerable<IWebhookDeliveryAttempt> IWebhookDeliveryResult.DeliveryAttempts => DeliveryAttempts;

		public virtual List<MongoDbWebhookDeliveryAttempt> DeliveryAttempts { get; set; }

		public string TenantId { get; set; }
	}
}
