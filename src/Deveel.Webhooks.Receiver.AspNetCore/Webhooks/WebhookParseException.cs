using System;

namespace Deveel.Webhooks {
	public sealed class WebhookParseException : WebhookException {
		public WebhookParseException() {
		}

		public WebhookParseException(string message) : base(message) {
		}

		public WebhookParseException(string message, Exception innerException) : base(message, innerException) {
		}
	}
}
