using System;

namespace Deveel.Webhooks.Storage {
	public interface IWebhookSubscriptionStoreProvider<TSubscription>
		where TSubscription : class, IWebhookSubscription {
		IWebhookSubscriptionStore<TSubscription> GetTenantRepository(string tenantId);
	}

	public interface IWebhookSubscriptionStoreProvider :
		IWebhookSubscriptionStoreProvider<IWebhookSubscription> {
	}
}