using System;

namespace Deveel.Webhooks {
	public interface IWebhookSender<TWebhook> where TWebhook : class {
		Task<WebhookDeliveryResult<TWebhook>> SendAsync(WebhookDestination receiver, TWebhook webhook, CancellationToken cancellationToken = default);
	}
}
