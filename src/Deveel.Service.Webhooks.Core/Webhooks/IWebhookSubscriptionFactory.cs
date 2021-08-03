using System;

namespace Deveel.Webhooks {
	public interface IWebhookSubscriptionFactory {
		IWebhookSubscription CreateSubscription(WebhookSubscriptionInfo subscriptionInfo);
	}
}
