using System;
using System.Collections.Generic;
using System.Linq;

using Deveel.Filters;

namespace Deveel.Webhooks.Memory {
	class WebhookSubscriptionObjectFactory : IWebhookSubscriptionFactory {
		public IWebhookSubscription Create(WebhookSubscriptionInfo subscriptionInfo) {
			return new InMemoryWebhookSubscription {
				Name = subscriptionInfo.Name,
				DestinationUrl = subscriptionInfo.DestinationUrl.ToString(),
				EventTypes = subscriptionInfo.EventTypes,
				RetryCount = subscriptionInfo.RetryCount,
				Secret = subscriptionInfo.Secret,
				IsActive = subscriptionInfo.Active,
				Filters = subscriptionInfo.Filters?.Select(GetFilter).ToList(),
				Headers = subscriptionInfo.Headers == null ? null : new Dictionary<string, string>(subscriptionInfo.Headers),
				Metadata = subscriptionInfo == null ? null : new Dictionary<string, object>(subscriptionInfo.Metadata)
			};
		}

		private static WebhookFilter GetFilter(IWebhookFilter filter)
			=> new WebhookFilter(filter.Expression, filter.Format);
	}
}
