using System;
using System.Collections.Generic;
using System.Linq;

using Deveel.Events;
using Deveel.Filters;

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
				Filters = GetFilters(subscriptionInfo.Filter),
				Metadata = subscriptionInfo.Metadata != null
					? new Dictionary<string, object>(subscriptionInfo.Metadata)
					: new Dictionary<string, object>()
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
