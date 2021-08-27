using System;
using System.Collections.Generic;
using System.Linq;

using Deveel.Events;

namespace Deveel.Webhooks {
	public sealed class MongoDbWebhookSubscriptionFactory : IWebhookSubscriptionFactory {
		public IWebhookSubscription CreateSubscription(WebhookSubscriptionInfo subscriptionInfo) {
			return new WebhookSubscriptionDocument {
				Name = subscriptionInfo.Name,
				EventTypes = subscriptionInfo.EventTypes?.ToList(),
				DestinationUrl = subscriptionInfo.DestinationUrl.ToString(),
				RetryCount = subscriptionInfo.RetryCount,
				Secret = subscriptionInfo.Secret,
				Headers = subscriptionInfo.Headers != null
					? new Dictionary<string, string>(subscriptionInfo.Headers)
					: null,
				Filter = subscriptionInfo.Filter?.ToString(),
				Metadata = subscriptionInfo.Metadata != null
					? new Dictionary<string, object>(subscriptionInfo.Metadata)
					: new Dictionary<string, object>()
			};
		}
	}
}
