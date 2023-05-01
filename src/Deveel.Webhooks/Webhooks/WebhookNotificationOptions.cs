using System;

namespace Deveel.Webhooks {
	/// <summary>
	/// Provides configuration options for the notification of webhooks.
	/// </summary>
	/// <typeparam name="TWebhook"></typeparam>
	public class WebhookNotificationOptions<TWebhook> {
		/// <summary>
		/// Gets or sets the number of parallel threads that will be used
		/// to send the notifications.
		/// </summary>
		public int ParallelThreadCount { get; set; } = Environment.ProcessorCount - 1;
	}
}
