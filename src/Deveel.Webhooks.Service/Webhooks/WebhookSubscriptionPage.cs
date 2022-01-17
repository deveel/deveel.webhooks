using System;
using System.Collections.Generic;

namespace Deveel.Webhooks {
	public sealed class WebhookSubscriptionPage<TSubscription> where TSubscription : class, IWebhookSubscription {
		public WebhookSubscriptionPage(WebhookSubscriptionQuery<TSubscription> query, int totalCount, IEnumerable<TSubscription> subscriptions = null) {
			if (totalCount < 0)
				throw new ArgumentOutOfRangeException(nameof(totalCount), "The total count must be equal or greater than zero");

			Query = query ?? throw new ArgumentNullException(nameof(query));
			TotalCount = totalCount;
			Subscriptions = subscriptions;
		}

		public WebhookSubscriptionQuery<TSubscription> Query { get; }

		public IEnumerable<TSubscription> Subscriptions { get; set; }

		public int TotalCount { get; }

		public int PageSize => Query.PageSize;

		public int Offset => Query.Offset;

		public int TotalPages => (int)Math.Ceiling((double)TotalCount / Query.PageSize);
	}
}
