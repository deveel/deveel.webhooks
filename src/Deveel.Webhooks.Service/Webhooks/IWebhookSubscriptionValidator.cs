using System;
using System.Threading;
using System.Threading.Tasks;

namespace Deveel.Webhooks {
	public interface IWebhookSubscriptionValidator<TSubscription> where TSubscription : class, IWebhookSubscription {
		Task<WebhookValidationResult> ValidateAsync(WebhookSubscriptionManager<TSubscription> manager, TSubscription subscription, CancellationToken cancellationToken);
	}
}
