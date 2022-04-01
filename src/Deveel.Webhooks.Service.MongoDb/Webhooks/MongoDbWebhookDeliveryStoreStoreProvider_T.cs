using System;

using Deveel.Data;

using Microsoft.Extensions.Options;

namespace Deveel.Webhooks {
	public class MongoDbWebhookDeliveryResultStoreProvider<TResult> : MongoDbStoreProviderBase<TResult>, IWebhookDeliveryResultStoreProvider<TResult>
		where TResult : MongoDbWebhookDeliveryResult {
		public MongoDbWebhookDeliveryResultStoreProvider(IOptions<MongoDbOptions> options) : base(options) {
		}

		public MongoDbWebhookDeliveryResultStoreProvider(MongoDbOptions options) : base(options) {
		}

		protected override MongoDbStoreBase<TResult> CreateStore(MongoDbOptions options) 
			=> new MongoDbWebhookDeliveryResultStore<TResult>(options);

		public MongoDbWebhookDeliveryResultStore<TResult> GetStore(string tenantId)
			=> (MongoDbWebhookDeliveryResultStore<TResult>) CreateStore(tenantId);

		IWebhookDeliveryResultStore<TResult> IWebhookDeliveryResultStoreProvider<TResult>.GetTenantStore(string tenantId)
			=> GetStore(tenantId);
	}
}
