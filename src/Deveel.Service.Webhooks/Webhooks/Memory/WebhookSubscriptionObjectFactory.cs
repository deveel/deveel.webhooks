using System;
using System.Collections.Generic;
using System.Linq;

using Deveel.Filters;

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
				Filters = subscriptionInfo.Filters?.Select(GetFilter).ToList(),
				Headers = subscriptionInfo.Headers == null ? null : new Dictionary<string, string>(subscriptionInfo.Headers),
				Metadata = subscriptionInfo == null ? null : new Dictionary<string, object>(subscriptionInfo.Metadata)
			};
		}

		private static InMemoryWebhookFilter GetFilter(IWebhookFilter filter)
			=> new InMemoryWebhookFilter {
				Provider = filter.Provider,
				Expression = filter.Expression,
				ExpressionFormat = filter.ExpressionFormat
			};
	}
}
