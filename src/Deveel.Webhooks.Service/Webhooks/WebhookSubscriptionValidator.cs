using System;
using System.Threading;
using System.Threading.Tasks;

namespace Deveel.Webhooks {
	public class WebhookSubscriptionValidator<TSubscription> : IWebhookSubscriptionValidator<TSubscription> where TSubscription : class, IWebhookSubscription {
		public virtual Task<WebhookValidationResult> ValidateAsync(WebhookSubscriptionManager<TSubscription> manager, TSubscription subscription, CancellationToken cancellationToken) {
			cancellationToken.ThrowIfCancellationRequested();

			var result = Validate(manager, subscription);

			return Task.FromResult(result);
		}

		public virtual WebhookValidationResult Validate(WebhookSubscriptionManager<TSubscription> manager, TSubscription subscription) {
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
