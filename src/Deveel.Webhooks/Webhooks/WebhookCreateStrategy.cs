namespace Deveel.Webhooks {
	/// <summary>
	/// The strategy to use when creating a webhook instance
	/// from a notification.
	/// </summary>
	public enum WebhookCreateStrategy {
		/// <summary>
		/// The factory will create a single webhook instance
		/// for each event in the notification.
		/// </summary>
		OnePerEvent = 1,

		/// <summary>
		/// The factory will create a single webhook instance
		/// for all the events in the notification.
		/// </summary>
		OnePerNotification = 2
	}
}
