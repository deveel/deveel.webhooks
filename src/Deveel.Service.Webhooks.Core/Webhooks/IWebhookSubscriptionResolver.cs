using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Deveel.Webhooks {
	public interface IWebhookSubscriptionResolver {
		Task<IList<IWebhookSubscription>> ResolveSubscriptionsAsync(string tenantId, string eventType, CancellationToken cancellationToken);
	}
}
