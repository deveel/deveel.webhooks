using System;
using System.Threading.Tasks;
using System.Threading;

namespace Deveel.Webhooks {
	public interface IWebhookDeliveryResultPagedStore<TResult> : IWebhookDeliveryResultStore<TResult> where TResult : class, IWebhookDeliveryResult {
		Task<PagedResult<TResult>> GetPageAsync(PagedQuery<TResult> query, CancellationToken cancellationToken);
	}
}
