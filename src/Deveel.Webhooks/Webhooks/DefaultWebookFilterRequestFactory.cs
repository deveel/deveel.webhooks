using System;
using System.Linq;

namespace Deveel.Webhooks {
	public class DefaultWebookFilterRequestFactory : IWebhookFilterRequestFactory {
		public virtual WebhookFilterRequest CreateRequest(IWebhookSubscription subscription) {
			if (subscription.Filters == null ||
				!subscription.Filters.Any())
				return null;

			var formats = subscription.Filters?.Select(x => x.Format).Distinct().ToList();
			if (formats.Count > 1)
				throw new InvalidOperationException("The subscription has filters with multiple formats");

			var request = new WebhookFilterRequest(formats[0]);

			foreach (var filter in subscription.Filters) {
				request.AddFilter(filter.Expression);
			}

			return request;
		}
	}
}
