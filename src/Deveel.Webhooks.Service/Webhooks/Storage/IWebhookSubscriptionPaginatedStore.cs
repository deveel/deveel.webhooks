using System;
using System.Threading;
using System.Threading.Tasks;

using Deveel.Data;

namespace Deveel.Webhooks.Storage {
	public interface IWebhookSubscriptionPaginatedStore<TSubscription> : IWebhookSubscriptionStore<TSubscription>
		where TSubscription : class, IWebhookSubscription {
		Task<PagedResult<TSubscription>> GetPageAsync(PagedQuery<TSubscription> query, CancellationToken cancellationToken);
	}
}
