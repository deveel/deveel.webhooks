using System;
using System.Threading.Tasks;
using System.Threading;

namespace Deveel.Webhooks {
	public interface IMultiTenantWebhookSubscriptionValidator<TSubscription>
		where TSubscription : class, IWebhookSubscription {
		Task<WebhookValidationResult> ValidateAsync(IMultiTenantWebhookSubscriptionManager<TSubscription> manager, string tenantId, TSubscription subscription, CancellationToken cancellationToken);
	}
}
