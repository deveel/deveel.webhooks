using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Deveel.Webhooks {
	public class TestSubscriptionResolver : IWebhookSubscriptionResolver {
		private readonly IList<IWebhookSubscription> subscriptions = new List<IWebhookSubscription>();

		public void AddSubscription(IWebhookSubscription subscription) {
			subscriptions.Add(subscription);
		}


		public Task<IList<IWebhookSubscription>> ResolveSubscriptionsAsync(string eventType, bool activeOnly, CancellationToken cancellationToken) {
			var result = subscriptions.Where(x => (!activeOnly || x.Status == WebhookSubscriptionStatus.Active) &&
				(x.EventTypes?.Any(y => y == eventType) ?? false))
				.ToList();

			return Task.FromResult<IList<IWebhookSubscription>>(result);
		}

	}
}
