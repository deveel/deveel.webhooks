using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Deveel.Webhooks.Caching {
	/// <summary>
	/// A cache of webhook subscriptions that optimizes the
	/// read access to the entities
	/// </summary>
	/// <typeparam name="TSubscription"></typeparam>
	public interface IWebhookSubscriptionCache {
		Task<IWebhookSubscription> GetByIdAsync(string tenantId, string id, CancellationToken cancellationToken);

		Task<IList<IWebhookSubscription>> GetByEventTypeAsync(string tenantId, string eventType, CancellationToken cancellationToken);

		Task RemoveByEventTypeAsync(string tenantId, string eventType, CancellationToken cancellationToken);

		Task SetAsync(IWebhookSubscription subscription, CancellationToken cancellationToken);

		Task RemoveAsync(IWebhookSubscription subscription, CancellationToken cancellationToken);
	}
}
