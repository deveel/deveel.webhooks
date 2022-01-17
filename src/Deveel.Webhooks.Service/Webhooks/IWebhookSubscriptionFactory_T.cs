using System;

namespace Deveel.Webhooks {
	public interface IWebhookSubscriptionFactory<TSubscription> where TSubscription : class, IWebhookSubscription {
		TSubscription Create(WebhookSubscriptionInfo subscriptionInfo);
	}
}
