using System;

namespace Deveel.Webhooks {
	/// <summary>
	/// An exception that denotes an error during the execution
	/// of the webhook service
	/// </summary>
	public class WebhookServiceException : WebhookException {
		public WebhookServiceException() {
		}

		public WebhookServiceException(string message) : base(message) {
		}

		public WebhookServiceException(string message, Exception innerException) : base(message, innerException) {
		}
	}
}
