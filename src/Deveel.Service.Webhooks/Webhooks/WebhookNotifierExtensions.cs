using System;
using System.Threading;
using System.Threading.Tasks;

using Deveel.Events;

namespace Deveel.Webhooks {
	public static class WebhookNotifierExtensions {
		public static Task<WebhookNotificationResult> NotifyAsync(this IWebhookNotifier notifier, string tenantId, IEvent @event, CancellationToken cancellationToken = default)
			=> notifier.NotifyAsync(tenantId, @event.AsEventInfo(), cancellationToken);
	}
}
