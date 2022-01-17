using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Deveel.Webhooks.Storage {
	public static class WebhookSubscriptionStoreExtensions {
		public static Task<string> CreateAsync<TSubscription>(this IWebhookSubscriptionStoreProvider<TSubscription> provider, string tenantId,
			TSubscription subscription, CancellationToken cancellationToken = default)
			where TSubscription : class, IWebhookSubscription
			=> provider.GetTenantStore(tenantId).CreateAsync(subscription, cancellationToken);

		public static Task<bool> DeleteAsync<TSubscription>(this IWebhookSubscriptionStoreProvider<TSubscription> provider, string tenantId,
			TSubscription subscription, CancellationToken cancellationToken = default)
			where TSubscription : class, IWebhookSubscription
			=> provider.GetTenantStore(tenantId).DeleteAsync(subscription, cancellationToken);

		public static Task<bool> UpdateAsync<TSubscription>(this IWebhookSubscriptionStoreProvider<TSubscription> provider, string tenantId,
			TSubscription subscription, CancellationToken cancellationToken = default)
			where TSubscription : class, IWebhookSubscription
			=> provider.GetTenantStore(tenantId).UpdateAsync(subscription, cancellationToken);

		public static Task<TSubscription> GetByIdAsync<TSubscription>(this IWebhookSubscriptionStoreProvider<TSubscription> provider, string tenantId,
			string id, CancellationToken cancellationToken = default)
			where TSubscription : class, IWebhookSubscription
			=> provider.GetTenantStore(tenantId).GetByIdAsync(id, cancellationToken);

		public static Task<IList<TSubscription>> GetByEventTypeAsync<TSubscription>(this IWebhookSubscriptionStoreProvider<TSubscription> provider, string tenantId,
			string eventType, bool activeOnly, CancellationToken cancellationToken = default)
			where TSubscription : class, IWebhookSubscription
			=> provider.GetTenantStore(tenantId).GetByEventTypeAsync(eventType, activeOnly, cancellationToken);

		public static Task SetStateAsync<TSubscripton>(this IWebhookSubscriptionStoreProvider<TSubscripton> provider, string tenantId, TSubscripton subscripton, WebhookSubscriptionStateInfo stateInfo, CancellationToken cancellationToken = default)
			where TSubscripton : class, IWebhookSubscription
			=> provider.GetTenantStore(tenantId).SetStateAsync(subscripton, stateInfo, cancellationToken);
	}
}
