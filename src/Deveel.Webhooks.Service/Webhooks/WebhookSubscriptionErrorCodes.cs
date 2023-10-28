namespace Deveel.Webhooks {
	/// <summary>
	/// Provides a list of the possible error codes
	/// that can be returned by an operation of
	/// management of webhook subscriptions.
	/// </summary>
	public static class WebhookSubscriptionErrorCodes {
		/// <summary>
		/// An unknown error occurred while managing webhook subscriptions.
		/// </summary>
		public const string UnknownError = "WEBHOOK_SUBSCRIPTION_UNKNOWN_ERROR";

		/// <summary>
		/// The subscription is invalid.
		/// </summary>
		public const string SubscriptionInvalid = "WEBHOOK_SUBSCRIPTION_INVALID";

		/// <summary>
		/// The new status of the subscription is invalid.
		/// </summary>
		public const string InvalidStatus = "WEBHOOK_SUBSCRIPTION_STATUS_INVALID";

		/// <summary>
		/// The subscription was not found.
		/// </summary>
		public const string SubscriptionNotFound = "WEBHOOK_SUBSCRIPTION_NOT_FOUND";
	}
}
