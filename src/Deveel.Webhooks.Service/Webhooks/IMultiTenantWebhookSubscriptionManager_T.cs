using System;
using System.Threading.Tasks;
using System.Threading;

namespace Deveel.Webhooks {
	public interface IMultiTenantWebhookSubscriptionManager<TSubscription> 
		where TSubscription : class, IWebhookSubscription {
		Task<string> AddSubscriptionAsync(string tenantId, WebhookSubscriptionInfo subscription, CancellationToken cancellationToken);

		Task<string> AddSubscriptionAsync(string tenantId, TSubscription subscription, CancellationToken cancellationToken);

		Task<bool> EnableSubscriptionAsync(string tenantId, string subscriptionId, CancellationToken cancellationToken);

		Task<bool> EnableSubscriptionAsync(string tenantId, TSubscription subscription, CancellationToken cancellationToken);

		Task<bool> DisableSubscriptionAsync(string tenantId, string subscriptionId, CancellationToken cancellationToken);

		Task<bool> DisableSubscriptionAsync(string tenantId, TSubscription subscription, CancellationToken cancellationToken);

		Task<TSubscription> GetSubscriptionAsync(string tenantId, string subscriptionId, CancellationToken cancellationToken);

		Task<PagedResult<TSubscription>> GetSubscriptionsAsync(string tenantId, PagedQuery<TSubscription> query, CancellationToken cancellationToken);

		Task<bool> RemoveSubscriptionAsync(string tenantId, string subscriptionId, CancellationToken cancellationToken);

		Task<bool> RemoveSubscriptionAsync(string tenantId, TSubscription subscription, CancellationToken cancellationToken);

		Task<int> CountAllAsync(string tenantId, CancellationToken cancellationToken);
	}
}
