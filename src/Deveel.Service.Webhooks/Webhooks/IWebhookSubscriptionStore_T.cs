using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Threading;

using Deveel.Data;

namespace Deveel.Webhooks {
	public interface IWebhookSubscriptionStore<TSubscription> : IStore<TSubscription>
		where TSubscription : class, IWebhookSubscription {
		Task<PaginatedResult<TSubscription>> GetPageByMetadataAsync(string key, object value, PageRequest page, CancellationToken cancellationToken);

		Task<bool> MetadataExistsAsync(string key, object value, CancellationToken cancellationToken);

		Task SetStateAsync(TSubscription subscription, bool active, CancellationToken cancellationToken);

		Task<IList<TSubscription>> GetByEventTypeAsync(string eventType, CancellationToken cancellationToken);
	}
}
