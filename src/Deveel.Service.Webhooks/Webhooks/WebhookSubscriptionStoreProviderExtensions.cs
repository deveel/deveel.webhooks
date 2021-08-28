using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;

using Deveel.Data;
using Deveel.Webhooks;

namespace Deveel.Webhooks {
	public static class WebhookSubscriptionStoreProviderExtensions {
		public static Task<PaginatedResult<IWebhookSubscription>> GetPageByMetadataAsync(this IWebhookSubscriptionStoreProvider provider, string tenantId, string key, object value, PageRequest page, CancellationToken cancellationToken)
			=> provider.GetStore(tenantId).GetPageByMetadataAsync(key, value, page, cancellationToken);

		public static Task<bool> MetadataExistsAsync<TSubscription>(this IWebhookSubscriptionStoreProvider provider, string tenantId, string key, object value, CancellationToken cancellationToken)
			where TSubscription : class, IWebhookSubscription
			=> provider.GetStore(tenantId).MetadataExistsAsync(key, value, cancellationToken);

		public static Task<IList<IWebhookSubscription>> GetByEventTypeAsync(this IWebhookSubscriptionStoreProvider provider, string tenantId, string eventType, CancellationToken cancellationToken)
			=> provider.GetStore(tenantId).GetByEventTypeAsync(eventType, cancellationToken);

		public static Task SetState<TSubscription>(this IWebhookSubscriptionStoreProvider<TSubscription> provider, string tenantId,
			TSubscription subscription, bool active, CancellationToken cancellationToken)
			where TSubscription : class, IWebhookSubscription
			=> provider.GetStore(tenantId).SetStateAsync(subscription, active, cancellationToken);

		public static Task<PaginatedResult<TSubscription>> GetPageByMetadataAsync<TSubscription>(this IWebhookSubscriptionStoreProvider<TSubscription> provider, 
			string tenantId, string key, object value, PageRequest page, CancellationToken cancellationToken)
			where TSubscription : class, IWebhookSubscription
			=> provider.GetStore(tenantId).GetPageByMetadataAsync(key, value, page, cancellationToken);

		public static Task<IList<TSubscription>> GetByEventTypeAsync<TSubscription>(this IWebhookSubscriptionStoreProvider<TSubscription> provider, 
			string tenantId, string eventType, CancellationToken cancellationToken)
			where TSubscription : class, IWebhookSubscription
			=> provider.GetStore(tenantId).GetByEventTypeAsync(eventType, cancellationToken);

	}
}
