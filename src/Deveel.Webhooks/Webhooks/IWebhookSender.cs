using System;
using System.Threading;
using System.Threading.Tasks;

namespace Deveel.Webhooks {
	public interface IWebhookSender {
		Task<WebhookDeliveryResult> SendAsync(IWebhook webhook, CancellationToken cancellationToken);
	}
}
