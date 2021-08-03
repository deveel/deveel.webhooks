using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

using Deveel.Webhooks;

namespace Deveel.Events {
	public static class WebhookStoreExtensions {
		public static Task SetStateAsync(this IWebhookSubscriptionStoreProvider provider, string tenantId, IWebhookSubscription subsctiption, bool active, CancellationToken cancellationToken)
			=> provider.GetStore(tenantId).SetStateAsync(subsctiption, active, cancellationToken);
	}
}
