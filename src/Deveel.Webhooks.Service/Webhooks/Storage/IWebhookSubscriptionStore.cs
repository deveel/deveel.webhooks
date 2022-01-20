using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

using Deveel.Data;

namespace Deveel.Webhooks.Storage {
	public interface IWebhookSubscriptionStore<TSubscription> : IStore<TSubscription>
		where TSubscription : class, IWebhookSubscription {

		Task<IList<TSubscription>> GetByEventTypeAsync(string eventType, bool activeOnly, CancellationToken cancellationToken);

		Task SetStateAsync(TSubscription subscription, WebhookSubscriptionStateInfo stateInfo, CancellationToken cancellationToken);
	}

	public interface IWebhookSubscriptionStore : IWebhookSubscriptionStore<IWebhookSubscription> {
	}
}