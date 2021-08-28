using System;

using Deveel.Data;

namespace Deveel.Webhooks {
	public interface IWebhookSubscriptionStoreProvider : IWebhookSubscriptionStoreProvider<IWebhookSubscription> {
		new IWebhookSubscriptionStore GetStore(string tenantId);
	}
}
