using System;

namespace Deveel.Webhooks {
	public interface IWebhookSubscriptionManagerProvider<TSubscription> where TSubscription : class, IWebhookSubscription {
		IWebhookSubscriptionManager<TSubscription> GetManager(string tenantId);
	}
}
