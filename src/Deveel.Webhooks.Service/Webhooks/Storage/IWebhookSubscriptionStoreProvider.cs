using System;

namespace Deveel.Webhooks.Storage {
	public interface IWebhookSubscriptionStoreProvider<TSubscription>
		where TSubscription : class, IWebhookSubscription {
		IWebhookSubscriptionStore<TSubscription> GetTenantStore(string tenantId);
	}

	public interface IWebhookSubscriptionStoreProvider :
		IWebhookSubscriptionStoreProvider<IWebhookSubscription> {
	}
}