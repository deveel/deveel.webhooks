using System;
using System.Collections.Generic;

namespace Deveel.Webhooks.Memory {
	class WebhookSubscriptionObjectFactory : IWebhookSubscriptionFactory {
		public IWebhookSubscription CreateSubscription(WebhookSubscriptionInfo subscriptionInfo) {
			return new InMemoryWebhookSubscription {
				Name = subscriptionInfo.Name,
				DestinationUrl = subscriptionInfo.DestinationUrl.ToString(),
				EventTypes = subscriptionInfo.EventTypes,
				Id = subscriptionInfo.SubscriptionId,
				RetryCount = subscriptionInfo.RetryCount,
				Secret = subscriptionInfo.Secret,
				Filter = subscriptionInfo.Filter?.ToString(),
				Headers = subscriptionInfo.Headers == null ? null : new Dictionary<string, string>(subscriptionInfo.Headers),
				Metadata = subscriptionInfo == null ? null : new Dictionary<string, object>(subscriptionInfo.Metadata)
			};
		}
	}
}
