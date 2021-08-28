using System;

using Deveel.Events;
using Deveel.Webhooks;

namespace Deveel.Webhooks {
	public static class WebhookSubscriptionExtensions {
		public static bool Matches(this IWebhookSubscription subscription, IWebhook webhook)
			=> subscription.Matches("hook", webhook);
	}
}
