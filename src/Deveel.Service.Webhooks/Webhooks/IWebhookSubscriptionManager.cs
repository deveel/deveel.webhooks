using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

using Deveel.Data;

namespace Deveel.Webhooks {
	public interface IWebhookSubscriptionManager {
		Task<string> AddSubscriptionAsync(string tenantId, WebhookSubscriptionInfo subscription, CancellationToken cancellationToken);

		Task<bool> EnableSubscriptionAsync(string tenantId, string subscriptionId, CancellationToken cancellationToken);

		Task<bool> DisableSubscriptionAsync(string tenantId, string subscriptionId, CancellationToken cancellationToken);

		Task<IWebhookSubscription> GetSubscriptionAsync(string tenantId, string subscriptionId, CancellationToken cancellationToken);

		Task<PaginatedResult<IWebhookSubscription>> GetSubscriptionsAsync(string tenantId, PageRequest page, CancellationToken cancellationToken);

		Task<PaginatedResult<IWebhookSubscription>> GetSubscriptionsByMetadataAsync(string tenantId, string key, object value, PageRequest page, CancellationToken cancellationToken);

		Task<bool> RemoveSubscriptionAsync(string tenantId, string subscriptionId, CancellationToken cancellationToken);
	}
}
