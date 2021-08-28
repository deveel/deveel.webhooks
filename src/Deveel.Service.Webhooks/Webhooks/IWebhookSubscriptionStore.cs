using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

using Deveel.Data;

namespace Deveel.Webhooks {
	public interface IWebhookSubscriptionStore : IWebhookSubscriptionStore<IWebhookSubscription> {
	}
}
