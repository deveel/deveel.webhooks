using System;

namespace Deveel.Webhooks {
	public interface IWebhookSubscriptionRepositoryProvider<TSubscription>
		where TSubscription : class, IWebhookSubscription{
		IWebhookSubscriptionRepository<TSubscription> GetTenantRepository(string tenantId);
	}

	public interface IWebhookSubscriptionRepositoryProvider :
		IWebhookSubscriptionRepositoryProvider<IWebhookSubscription> {
	}
}
