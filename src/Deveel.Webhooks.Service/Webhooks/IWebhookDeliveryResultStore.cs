using System;
using System.Threading.Tasks;
using System.Threading;

namespace Deveel.Webhooks {
	public interface IWebhookDeliveryResultStore<TResult> where TResult : class, IWebhookDeliveryResult {

		Task<string> CreateAsync(TResult result, CancellationToken cancellationToken);

		Task<TResult> FindByIdAsync(string id, CancellationToken cancellationToken);

		Task<bool> DeleteAsync(TResult result, CancellationToken cancellationToken);

		Task<int> CountAllAsync(CancellationToken cancellationToken);

		Task<TResult> FindByWebhookIdAsync(string webhookId, CancellationToken cancellationToken);
	}
}
