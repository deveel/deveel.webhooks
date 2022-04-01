using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Deveel.Webhooks {
	public interface IWebhookSubscriptionTenantStore<TSubscription> : IWebhookSubscriptionStore<TSubscription> where TSubscription : class, IWebhookSubscription {
		string TenantId { get; }
	}
}
