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
				Filters = GetFilters(subscriptionInfo.Filter),
				Headers = subscriptionInfo.Headers == null ? null : new Dictionary<string, string>(subscriptionInfo.Headers),
				Metadata = subscriptionInfo == null ? null : new Dictionary<string, object>(subscriptionInfo.Metadata)
			};
		}

		private IList<string> GetFilters(object filter) {
			if (filter == null)
				return new List<string>();

			if (filter is string s)
				return new[] { s };
			if (filter is IEnumerable<string> strings)
				return strings?.ToList();
			if (filter is IFilter f)
				return new[] { f?.ToFilterString() };
			if (filter is IEnumerable<IFilter> filters)
				return filters?.Select(f => f.ToFilterString()).ToArray();

			return null;
		}
	}
}
