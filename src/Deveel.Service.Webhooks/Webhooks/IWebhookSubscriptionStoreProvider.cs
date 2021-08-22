using System;

using Deveel.Data;

namespace Deveel.Webhooks {
	public interface IWebhookSubscriptionStoreProvider : IStoreProvider<IWebhookSubscription> {
		new IWebhookSubscriptionStore GetStore(string tenantId);
	}
}
