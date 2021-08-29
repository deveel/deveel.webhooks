using System;
using System.Linq;

namespace Deveel.Webhooks {
	public class DefaultWebookFilterRequestFactory : IWebhookFilterRequestFactory {
		public virtual WebhookFilterRequest CreateRequest(IWebhookSubscription subscription) {
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
