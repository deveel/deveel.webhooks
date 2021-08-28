using System;
using System.Threading;
using System.Threading.Tasks;

namespace Deveel.Webhooks {
	public interface IWebhookNotifier {
		Task<WebhookNotificationResult> NotifyAsync(string tenantId, EventInfo eventInfo, CancellationToken cancellationToken);
	}
}
