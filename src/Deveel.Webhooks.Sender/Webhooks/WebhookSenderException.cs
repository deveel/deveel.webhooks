namespace Deveel.Webhooks {
	public class WebhookSenderException : Exception {
		public WebhookSenderException() {
		}

		public WebhookSenderException(string? message) : base(message) {
		}

		public WebhookSenderException(string? message, Exception? innerException) : base(message, innerException) {
		}
	}
}
