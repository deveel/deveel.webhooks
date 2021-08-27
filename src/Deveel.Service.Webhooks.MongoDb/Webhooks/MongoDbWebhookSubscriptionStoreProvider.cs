using System;

using Deveel.Data;

using Microsoft.Extensions.Options;

namespace Deveel.Webhooks {
	class MongoDbWebhookSubscriptionStoreProvider : MongoDbStoreProviderBase<WebhookSubscriptionDocument>,
														   IWebhookSubscriptionStoreProvider {
		public MongoDbWebhookSubscriptionStoreProvider(MongoDbOptions<WebhookSubscriptionDocument> baseOptions) : base(baseOptions) {
		}

		public MongoDbWebhookSubscriptionStoreProvider(IOptions<MongoDbOptions<WebhookSubscriptionDocument>> baseOptions) : base(baseOptions) {
		}

		protected override MongoDbEntityStore<WebhookSubscriptionDocument> CreateStore(MongoDbOptions<WebhookSubscriptionDocument> options)
			=> new MongoDbWebhookSubscriptionStrore(options);

		public new MongoDbWebhookSubscriptionStrore GetStore(string appId) => (MongoDbWebhookSubscriptionStrore)base.GetStore(appId);

		IWebhookSubscriptionStore IWebhookSubscriptionStoreProvider.GetStore(string appId)
			=> GetStore(appId);

		IStore<IWebhookSubscription> IStoreProvider<IWebhookSubscription>.GetStore(string tenantId)
			=> GetStore(tenantId);
	}
}
