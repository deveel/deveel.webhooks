using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Deveel.Webhooks {
	public interface IWebhookSubscriptionRepository<TSubscription> 
		where TSubscription : class, IWebhookSubscription {
		Task<string> CreateAsync(TSubscription subscription, CancellationToken cancellationToken);

		Task<TSubscription> GetByIdAsync(string id, CancellationToken cancellationToken);

		Task<IList<TSubscription>> GetByEventTypeAsync(string eventType, CancellationToken cancellationToken);

		Task<bool> DeleteAsync(TSubscription subscription, CancellationToken cancellationToken);

		Task<bool> UpdateAsync(TSubscription subscription, CancellationToken cancellationToken);

		Task<IList<TSubscription>> GetListAsync(int offset, int count, CancellationToken cancellationToken);

		Task<int> CountAllAsync(CancellationToken cancellationToken);
	}

	public interface IWebhookSubscriptionRepository : IWebhookSubscriptionRepository<IWebhookSubscription> {
	}
}
