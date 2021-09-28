using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;

using Deveel.Data;
using Deveel.Webhooks;

namespace Deveel.Webhooks {
	public static class WebhookSubscriptionStoreProviderExtensions {
		public static Task<IList<IWebhookSubscription>> GetByEventTypeAsync(this IWebhookSubscriptionStoreProvider provider, string tenantId, string eventType, bool activeOnly, CancellationToken cancellationToken)
			=> provider.GetStore(tenantId).GetByEventTypeAsync(eventType, activeOnly, cancellationToken);

		public static Task SetState<TSubscription>(this IWebhookSubscriptionStoreProvider<TSubscription> provider, string tenantId,
			TSubscription subscription, bool active, CancellationToken cancellationToken)
			where TSubscription : class, IWebhookSubscription
			=> provider.GetStore(tenantId).SetStateAsync(subscription, active, cancellationToken);

		public static Task<IList<TSubscription>> GetByEventTypeAsync<TSubscription>(this IWebhookSubscriptionStoreProvider<TSubscription> provider, 
			string tenantId, string eventType, bool activeOnly, CancellationToken cancellationToken)
			where TSubscription : class, IWebhookSubscription
			=> provider.GetStore(tenantId).GetByEventTypeAsync(eventType, activeOnly, cancellationToken);

	}
}
