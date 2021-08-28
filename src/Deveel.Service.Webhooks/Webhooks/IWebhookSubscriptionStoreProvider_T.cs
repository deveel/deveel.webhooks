using System;

using Deveel.Data;

namespace Deveel.Webhooks {
	public interface IWebhookSubscriptionStoreProvider<TSubscription> : IStoreProvider<TSubscription> 
		where TSubscription : class, IWebhookSubscription {
		new IWebhookSubscriptionStore<TSubscription> GetStore(string tenantId);
	}
}
