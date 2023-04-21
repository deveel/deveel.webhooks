using System;

namespace Deveel.Webhooks {
	public interface IWebhookSubscriptionStoreProvider<TSubscription>
		where TSubscription : class, IWebhookSubscription {
		IWebhookSubscriptionStore<TSubscription> GetTenantStore(string tenantId);
	}
}