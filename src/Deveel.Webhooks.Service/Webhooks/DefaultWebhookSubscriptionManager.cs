using System;

using Deveel.Webhooks.Storage;

using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;

namespace Deveel.Webhooks {
	public class DefaultWebhookSubscriptionManager : DefaultWebhookSubscriptionManager<IWebhookSubscription>, IWebhookSubscriptionManager {
		public DefaultWebhookSubscriptionManager(IWebhookSubscriptionStoreProvider subscriptionStore,
			IWebhookSubscriptionFactory subscriptionFactory)
			: this(subscriptionStore, subscriptionFactory, NullLogger<DefaultWebhookSubscriptionManager>.Instance) {
		}

		public DefaultWebhookSubscriptionManager(IWebhookSubscriptionStoreProvider subscriptionStore,
			IWebhookSubscriptionFactory subscriptionFactory,
			ILogger<DefaultWebhookSubscriptionManager> logger)
			: base(subscriptionStore, subscriptionFactory, logger) {
		}
	}
}
