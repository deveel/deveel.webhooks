using System;

using Deveel.Webhooks.Storage;

using Microsoft.Extensions.Options;

namespace Deveel.Webhooks {
	class MongoDbWebhookSubscriptionStoreProvider : MongoDbWebhookStoreProviderBase<WebhookSubscriptionDocument, IWebhookSubscription>,
														   IWebhookSubscriptionStoreProvider {
		public MongoDbWebhookSubscriptionStoreProvider(IOptions<MongoDbWebhookOptions> baseOptions)
			: base(baseOptions) {
		}

		public MongoDbWebhookSubscriptionStoreProvider(MongoDbWebhookOptions baseOptions) 
			: base(baseOptions) {
		}

		protected override MongoDbWebhookStoreBase<WebhookSubscriptionDocument, IWebhookSubscription> CreateStore(MongoDbWebhookOptions options)
			=> new MongoDbWebhookSubscriptionStrore(options);

		public MongoDbWebhookSubscriptionStrore GetStore(string tenantId) => (MongoDbWebhookSubscriptionStrore)CreateStore(tenantId);

		IWebhookSubscriptionStore<IWebhookSubscription> IWebhookSubscriptionStoreProvider<IWebhookSubscription>.GetTenantRepository(string tenantId)
			=> GetStore(tenantId);
	}
}
