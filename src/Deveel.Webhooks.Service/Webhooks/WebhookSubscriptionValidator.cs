using System;
using System.Threading;
using System.Threading.Tasks;

namespace Deveel.Webhooks {
	public class WebhookSubscriptionValidator<TSubscription> : IWebhookSubscriptionValidator<TSubscription> where TSubscription : class, IWebhookSubscription {
		public virtual Task<WebhookValidationResult> ValidateAsync(IWebhookSubscriptionManager<TSubscription> manager, TSubscription subscription, CancellationToken cancellationToken) => throw new NotImplementedException();
	}
}
