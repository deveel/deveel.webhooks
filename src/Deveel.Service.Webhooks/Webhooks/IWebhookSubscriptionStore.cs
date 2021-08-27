using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

using Deveel.Data;

namespace Deveel.Webhooks {
	public interface IWebhookSubscriptionStore : IStore<IWebhookSubscription> {
		Task<PaginatedResult<IWebhookSubscription>> GetPageByMetadataAsync(string key, object value, PageRequest page, CancellationToken cancellationToken);

		Task<bool> MetadataExistsAsync(string key, object value, CancellationToken cancellationToken);

		Task SetStateAsync(IWebhookSubscription subscription, bool active, CancellationToken cancellationToken);

		Task<IList<IWebhookSubscription>> GetByEventTypeAsync(string eventType, CancellationToken cancellationToken);
	}
}
