using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Deveel.Webhooks.Storage {
	public interface IWebhookSubscriptionStore<TSubscription>
		where TSubscription : class, IWebhookSubscription {
		Task<string> CreateAsync(TSubscription subscription, CancellationToken cancellationToken);

		Task<TSubscription> GetByIdAsync(string id, CancellationToken cancellationToken);

		Task<IList<TSubscription>> GetByEventTypeAsync(string eventType, bool activeOnly, CancellationToken cancellationToken);

		Task<bool> DeleteAsync(TSubscription subscription, CancellationToken cancellationToken);

		Task<bool> UpdateAsync(TSubscription subscription, CancellationToken cancellationToken);

		Task<int> CountAllAsync(CancellationToken cancellationToken);

		Task SetStateAsync(TSubscription subscription, WebhookSubscriptionStateInfo stateInfo, CancellationToken cancellationToken);
	}

	public interface IWebhookSubscriptionStore : IWebhookSubscriptionStore<IWebhookSubscription> {
	}
}