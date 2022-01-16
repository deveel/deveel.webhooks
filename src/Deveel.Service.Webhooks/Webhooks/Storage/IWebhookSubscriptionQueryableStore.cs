using System;
using System.Linq;

namespace Deveel.Webhooks.Storage {
	public interface IWebhookSubscriptionQueryableStore<TSubscription> : IWebhookSubscriptionStore<TSubscription>
		where TSubscription : class, IWebhookSubscription {
		IQueryable<TSubscription> AsQueryable();
	}
}
