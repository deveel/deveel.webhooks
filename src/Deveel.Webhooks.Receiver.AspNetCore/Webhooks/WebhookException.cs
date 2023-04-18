using System;

namespace Deveel.Webhooks {
	public class WebhookException : Exception {
		public WebhookException(string message, Exception innerException) 
			: base(message, innerException) {
		}

		public WebhookException(string message) 
			: base(message) {
		}

		public WebhookException() {

		}
	}
}
