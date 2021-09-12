using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Deveel.Webhooks {
	public static class WebhookSubscriptionRepositoryExtensions {
		public static Task<string> CreateAsync<TSubscription>(this IWebhookSubscriptionRepositoryProvider<TSubscription> provider, string tenantId,
			TSubscription subscription, CancellationToken cancellationToken = default)
			where TSubscription : class, IWebhookSubscription
			=> provider.GetTenantRepository(tenantId).CreateAsync(subscription, cancellationToken);

		public static Task<bool> DeleteAsync<TSubscription>(this IWebhookSubscriptionRepositoryProvider<TSubscription> provider, string tenantId,
			TSubscription subscription, CancellationToken cancellationToken = default)
			where TSubscription : class, IWebhookSubscription
			=> provider.GetTenantRepository(tenantId).DeleteAsync(subscription, cancellationToken);

		public static Task<bool> UpdateAsync<TSubscription>(this IWebhookSubscriptionRepositoryProvider<TSubscription> provider, string tenantId,
			TSubscription subscription, CancellationToken cancellationToken = default)
			where TSubscription : class, IWebhookSubscription
			=> provider.GetTenantRepository(tenantId).UpdateAsync(subscription, cancellationToken);

		public static Task<TSubscription> GetByIdAsync<TSubscription>(this IWebhookSubscriptionRepositoryProvider<TSubscription> provider, string tenantId,
			string id, CancellationToken cancellationToken = default)
			where TSubscription : class, IWebhookSubscription
			=> provider.GetTenantRepository(tenantId).GetByIdAsync(id, cancellationToken);

		public static Task<IList<TSubscription>> GetByEventTypeAsync<TSubscription>(this IWebhookSubscriptionRepositoryProvider<TSubscription> provider, string tenantId,
			string eventType, CancellationToken cancellationToken = default)
			where TSubscription : class, IWebhookSubscription
			=> provider.GetTenantRepository(tenantId).GetByEventTypeAsync(eventType, cancellationToken);

		public static Task<IList<TSubscription>> GetListAsync<TSubscription>(this IWebhookSubscriptionRepositoryProvider<TSubscription> provider, string tenantId,
			int offset, int count, CancellationToken cancellationToken = default)
			where TSubscription : class, IWebhookSubscription
			=> provider.GetTenantRepository(tenantId).GetListAsync(offset, count, cancellationToken);
	}
}
