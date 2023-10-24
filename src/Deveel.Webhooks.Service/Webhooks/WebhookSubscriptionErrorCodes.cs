namespace Deveel.Webhooks {
	/// <summary>
	/// Provides a list of the possible error codes
	/// that can be returned by an operation of
	/// management of webhook subscriptions.
	/// </summary>
	public static class WebhookSubscriptionErrorCodes {
		public const string UnknownError = "WEBHOOK_SUBSCRIPTION_UNKNOWN_ERROR";
		public const string SubscriptionInvalid = "WEBHOOK_SUBSCRIPTION_INVALID";
		public const string SubscriptionNotFound = "WEBHOOK_SUBSCRIPTION_NOT_FOUND";
	}
}
