using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Deveel.Webhooks {
    public class TestSubscriptionResolver : IWebhookSubscriptionResolver {
		private readonly Dictionary<string, List<IWebhookSubscription>> subscriptions;

		public TestSubscriptionResolver() {
			subscriptions = new Dictionary<string, List<IWebhookSubscription>>();
		}

		public void AddSubscription(IWebhookSubscription subscription) {
			if (!subscriptions.TryGetValue(subscription.TenantId!, out var list)) {
				list = new List<IWebhookSubscription>();
				subscriptions.Add(subscription.TenantId!, list);
			}

			list.Add(subscription);
		}

		public Task<IList<IWebhookSubscription>> ResolveSubscriptionsAsync(string tenantId, string eventType, bool activeOnly, CancellationToken cancellationToken) {
			if (String.IsNullOrWhiteSpace(tenantId))
				throw new ArgumentNullException(nameof(tenantId));

			if (!subscriptions.TryGetValue(tenantId, out var list)) {
				return Task.FromResult<IList<IWebhookSubscription>>(new List<IWebhookSubscription>());
			}

			var result = list.Where(x => (!activeOnly || x.Status == WebhookSubscriptionStatus.Active) &&
				(x.EventTypes?.Any(y => y == eventType) ?? false))
				.ToList();

			return Task.FromResult<IList<IWebhookSubscription>>(result);
		}
	}
}
