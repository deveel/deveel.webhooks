using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Deveel.Webhooks {
	public interface IWebhookSubscriptionManager : IWebhookSubscriptionManager<IWebhookSubscription> {
	}
}
