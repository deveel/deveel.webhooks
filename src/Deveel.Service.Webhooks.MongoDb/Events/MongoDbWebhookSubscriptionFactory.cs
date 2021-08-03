using System;
using System.Collections.Generic;
using System.Linq;

using Deveel.Webhooks;

namespace Deveel.Events {
	public sealed class MongoDbWebhookSubscriptionFactory : IWebhookSubscriptionFactory {
		public IWebhookSubscription CreateSubscription(WebhookSubscriptionInfo subscriptionInfo) {
			return new WebhookSubscriptionDocument {
				Name = subscriptionInfo.Name,
				EventType = subscriptionInfo.EventType,
				DestinationUrl = subscriptionInfo.DestinationUrl.ToString(),
				RetryCount = subscriptionInfo.RetryCount,
				Secret = subscriptionInfo.Secret,
				Headers = subscriptionInfo.Headers != null
					? new Dictionary<string, string>(subscriptionInfo.Headers)
					: null,
				Filters = subscriptionInfo.FilterExpressions == null
					? new List<string>()
					: new List<string>(subscriptionInfo.FilterExpressions),
				Metadata = subscriptionInfo.Metadata != null
					? new Dictionary<string, object>(subscriptionInfo.Metadata)
					: new Dictionary<string, object>()
			};
		}
	}
}
