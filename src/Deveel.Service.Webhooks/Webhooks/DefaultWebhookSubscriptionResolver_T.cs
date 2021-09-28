using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Deveel.Webhooks {
	public class DefaultWebhookSubscriptionResolver<TSubscription> : IWebhookSubscriptionResolver 
		where TSubscription : class, IWebhookSubscription { 
		private readonly IWebhookSubscriptionStoreProvider<TSubscription> storeProvider;

		public DefaultWebhookSubscriptionResolver(IWebhookSubscriptionStoreProvider<TSubscription> storeProvider) {
			this.storeProvider = storeProvider;
		}

		public async Task<IList<IWebhookSubscription>> ResolveSubscriptionsAsync(string tenantId, string eventType, bool activeOnly, CancellationToken cancellationToken) {
			var result = await storeProvider.GetByEventTypeAsync(tenantId, eventType, activeOnly, cancellationToken);
			return result.Cast<IWebhookSubscription>().ToList();
		}
	}
}
