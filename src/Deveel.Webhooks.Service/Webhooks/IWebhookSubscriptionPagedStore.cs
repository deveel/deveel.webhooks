using System;
using System.Threading.Tasks;
using System.Threading;

namespace Deveel.Webhooks {
	public interface IWebhookSubscriptionPagedStore<TSubscription> : IWebhookSubscriptionStore<TSubscription> where TSubscription : class, IWebhookSubscription {
		Task<PagedResult<TSubscription>> GetPageAsync(PagedQuery<TSubscription> query, CancellationToken cancellationToken);
	}
}
