using System;
using System.Threading.Tasks;
using System.Threading;

namespace Deveel.Webhooks {
	public interface IWebhookSubscriptionManager<TSubscription>
		where TSubscription : class, IWebhookSubscription {
		Task<string> AddSubscriptionAsync(string tenantId, string userId, WebhookSubscriptionInfo subscription, CancellationToken cancellationToken);

		Task<bool> EnableSubscriptionAsync(string tenantId, string userId, string subscriptionId, CancellationToken cancellationToken);

		Task<bool> DisableSubscriptionAsync(string tenantId, string userId, string subscriptionId, CancellationToken cancellationToken);

		Task<TSubscription> GetSubscriptionAsync(string tenantId, string subscriptionId, CancellationToken cancellationToken);

		Task<WebhookSubscriptionPage<TSubscription>> GetSubscriptionsAsync(string tenantId, WebhookSubscriptionQuery<TSubscription> query, CancellationToken cancellationToken);

		Task<bool> RemoveSubscriptionAsync(string tenantId, string userId, string subscriptionId, CancellationToken cancellationToken);

	}
}
