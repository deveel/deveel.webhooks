using System;

namespace Deveel.Webhooks {
	/// <summary>
	/// An exception that is thrown during the validation
	/// of a webhook subscription to be created or updated
	/// </summary>
	public class WebhookSubscriptionValidationException : WebhookServiceException {
		public WebhookSubscriptionValidationException(string[] errors = null) : this("The webhook subscription is invalid", errors) {
		}

		public WebhookSubscriptionValidationException(string message, string[] errors = null) : base(message) {
			Errors = errors;
		}

		/// <summary>
		/// Gets a set of errors during the validation of the subscription
		/// </summary>
		public string[] Errors { get; }
	}
}
