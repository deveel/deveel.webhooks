using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Deveel.Webhooks {
	public interface IWebhookSubscriptionStore<TSubscription> : IDisposable
		where TSubscription : class, IWebhookSubscription {


		Task<string> CreateAsync(TSubscription subscription, CancellationToken cancellationToken);

		Task<TSubscription> FindByIdAsync(string id, CancellationToken cancellationToken);

		Task<bool> DeleteAsync(TSubscription subscription, CancellationToken cancellationToken);

		Task<bool> UpdateAsync(TSubscription subscription, CancellationToken cancellationToken);

		Task<int> CountAllAsync(CancellationToken cancellationToken);


		Task<IList<TSubscription>> GetByEventTypeAsync(string eventType, bool activeOnly, CancellationToken cancellationToken);

		Task SetStateAsync(TSubscription subscription, WebhookSubscriptionStatus status, CancellationToken cancellationToken);
	}
}