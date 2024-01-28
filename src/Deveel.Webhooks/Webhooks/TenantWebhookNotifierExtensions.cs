using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Deveel.Webhooks {
	public static class TenantWebhookNotifierExtensions {
		public static Task<WebhookNotificationResult<TWebhook>> NotifyAsync<TWebhook>(this ITenantWebhookNotifier<TWebhook> notifier, string tenantId, EventInfo eventInfo, CancellationToken cancellationToken = default)
			where TWebhook : class {
			return notifier.NotifyAsync(tenantId, new EventNotification(eventInfo), cancellationToken);
		}
	}
}
