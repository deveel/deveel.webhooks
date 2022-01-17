using System;

namespace Deveel.Webhooks {
	public static class WebhookFilterExtensions {
		public static bool IsWildcard(this IWebhookFilter filter) => WebhookFilter.IsWildcard(filter.Expression);
	}
}
