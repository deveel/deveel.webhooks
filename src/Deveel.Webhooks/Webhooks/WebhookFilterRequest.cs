using System;
using System.Collections.Generic;
using System.Linq;

namespace Deveel.Webhooks {
	public sealed class WebhookFilterRequest {
		private readonly List<IWebhookFilter> filters;

		public WebhookFilterRequest() {
			filters = new List<IWebhookFilter>();
		}

		public IEnumerable<IWebhookFilter> Filters => filters.AsReadOnly();

		public bool IsEmpty => filters.Count == 0;

		public bool IsWildcard => filters.Count == 1 && filters[0].IsWildcard();

		public void AddFilter(IWebhookFilter filterInfo) {
			lock(filters) {
				filters.Add(filterInfo);
			}
		}

		public void AddFilter(string expression, string format = null)
			=> AddFilter(new WebhookFilter(expression, format));

		public static WebhookFilterRequest FromSubscription(IWebhookSubscription subscription) {
			if (subscription.Filters == null ||
				!subscription.Filters.Any())
				return null;

			var request = new WebhookFilterRequest();

			foreach (var filter in subscription.Filters) {
				request.AddFilter(filter);
			}

			return request;
		}
	}
}
