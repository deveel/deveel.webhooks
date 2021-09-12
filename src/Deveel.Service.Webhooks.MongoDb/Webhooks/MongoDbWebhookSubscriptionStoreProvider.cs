using System;

using Deveel.Data;

using Microsoft.Extensions.Options;

namespace Deveel.Webhooks {
	class MongoDbWebhookSubscriptionStoreProvider : MongoDbStoreProvider<WebhookSubscriptionDocument>,
														   IWebhookSubscriptionStoreProvider {
		public MongoDbWebhookSubscriptionStoreProvider(MongoDbOptions<WebhookSubscriptionDocument> baseOptions, IDocumentFieldMapper<WebhookSubscriptionDocument> fieldMapper = null) 
			: base(baseOptions, fieldMapper) {
			bool dummy = true;
		}

		public MongoDbWebhookSubscriptionStoreProvider(IOptions<MongoDbOptions<WebhookSubscriptionDocument>> baseOptions, IDocumentFieldMapper<WebhookSubscriptionDocument> fieldMapper = null) 
			: base(baseOptions, fieldMapper) {
			bool dummy = true;
		}

		protected override MongoDbStore<WebhookSubscriptionDocument> CreateStore(MongoDbOptions<WebhookSubscriptionDocument> options)
			=> new MongoDbWebhookSubscriptionStrore(options);

		public new MongoDbWebhookSubscriptionStrore GetStore(string tenantId) => (MongoDbWebhookSubscriptionStrore)base.GetStore(tenantId);

		IWebhookSubscriptionStore IWebhookSubscriptionStoreProvider.GetStore(string tenantId)
			=> GetStore(tenantId);

		IStore<IWebhookSubscription> IStoreProvider<IWebhookSubscription>.GetStore(string tenantId)
			=> GetStore(tenantId);

		IWebhookSubscriptionStore<IWebhookSubscription> IWebhookSubscriptionStoreProvider<IWebhookSubscription>.GetStore(string tenantId)
			=> GetStore(tenantId);
	}
}
