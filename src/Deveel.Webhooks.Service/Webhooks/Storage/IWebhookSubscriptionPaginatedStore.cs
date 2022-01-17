using System;
using System.Threading;
using System.Threading.Tasks;

namespace Deveel.Webhooks.Storage {
	public interface IWebhookSubscriptionPaginatedStore<TSubscription> : IWebhookSubscriptionStore<TSubscription>
		where TSubscription : class, IWebhookSubscription {
		Task<WebhookSubscriptionPage<TSubscription>> GetPageAsync(WebhookSubscriptionQuery<TSubscription> query, CancellationToken cancellationToken);
	}
}
