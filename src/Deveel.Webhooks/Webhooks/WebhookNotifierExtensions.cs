namespace Deveel.Webhooks {
	public static class WebhookNotifierExtensions {
		public static Task<WebhookNotificationResult<TWebhook>> NotifyAsync<TWebhook>(this IWebhookNotifier<TWebhook> notifier, EventInfo eventInfo, CancellationToken cancellationToken = default)
			where TWebhook : class {
			return notifier.NotifyAsync(new EventNotification(eventInfo), cancellationToken);
		}
	}
}
