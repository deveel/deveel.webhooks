using System;
using System.Threading.Tasks;
using System.Threading;

using Deveel.Data;

namespace Deveel.Webhooks {
	public interface IWebhookSubscriptionManager<TSubscription>
		where TSubscription : class, IWebhookSubscription {
		Task<string> AddSubscriptionAsync(string tenantId, string userId, WebhookSubscriptionInfo subscription, CancellationToken cancellationToken);

		Task<bool> EnableSubscriptionAsync(string tenantId, string userId, string subscriptionId, CancellationToken cancellationToken);

		Task<bool> DisableSubscriptionAsync(string tenantId, string userId, string subscriptionId, CancellationToken cancellationToken);

		Task<TSubscription> GetSubscriptionAsync(string tenantId, string subscriptionId, CancellationToken cancellationToken);

		Task<PaginatedResult<TSubscription>> GetSubscriptionsAsync(string tenantId, PageRequest page, CancellationToken cancellationToken);

		Task<bool> RemoveSubscriptionAsync(string tenantId, string userId, string subscriptionId, CancellationToken cancellationToken);

	}
}
