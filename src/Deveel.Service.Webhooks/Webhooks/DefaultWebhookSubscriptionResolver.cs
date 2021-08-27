using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

using Deveel.Events;

namespace Deveel.Webhooks {
	public class DefaultWebhookSubscriptionResolver : IWebhookSubscriptionResolver {
		private readonly IWebhookSubscriptionStoreProvider storeProvider;

		public DefaultWebhookSubscriptionResolver(IWebhookSubscriptionStoreProvider storeProvider) {
			this.storeProvider = storeProvider;
		}

		public Task<IList<IWebhookSubscription>> ResolveSubscriptionsAsync(string tenantId, string eventType, CancellationToken cancellationToken) {
			return storeProvider.GetByEventTypeAsync(tenantId, eventType, cancellationToken);
		}
	}
}
