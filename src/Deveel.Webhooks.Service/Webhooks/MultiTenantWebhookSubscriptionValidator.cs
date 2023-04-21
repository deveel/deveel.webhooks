using System;
using System.Threading;
using System.Threading.Tasks;

namespace Deveel.Webhooks {
	public class MultiTenantWebhookSubscriptionValidator<TSubscription> : IMultiTenantWebhookSubscriptionValidator<TSubscription>
		where TSubscription : class, IWebhookSubscription {
		public virtual Task<WebhookValidationResult> ValidateAsync(IMultiTenantWebhookSubscriptionManager<TSubscription> manager, string tenantId, TSubscription subscription, CancellationToken cancellationToken) {
			var result = Validate(manager, tenantId, subscription);

			return Task.FromResult(result);
		}

		public virtual WebhookValidationResult Validate(IMultiTenantWebhookSubscriptionManager<TSubscription> manager, string tenantId, TSubscription subscription) {
			if (String.IsNullOrWhiteSpace(tenantId))
				return WebhookValidationResult.Failed("The tenant ID is required in a multi-tenant context");

			if (String.IsNullOrWhiteSpace(subscription.DestinationUrl))
				return WebhookValidationResult.Failed("The destination URL of the webhook is missing");

			if (!Uri.TryCreate(subscription.DestinationUrl, UriKind.Absolute, out var url))
				return WebhookValidationResult.Failed("The destination URL format is invalid");

			// TODO: obtain the configuration of supported delivery channels: for the moment only HTTP(S)
			// in future implementations we might extend this to support more channels
			if (url.Scheme != Uri.UriSchemeHttps &&
				url.Scheme != Uri.UriSchemeHttp)
				return WebhookValidationResult.Failed($"URL scheme '{url.Scheme}' not supported");

			return WebhookValidationResult.Success;
		}
	}
}
