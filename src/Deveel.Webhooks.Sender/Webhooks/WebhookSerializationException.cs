using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Deveel.Webhooks {
	public sealed class WebhookSerializationException : WebhookSenderException {
		public WebhookSerializationException() {
		}

		public WebhookSerializationException(string? message) : base(message) {
		}

		public WebhookSerializationException(string? message, Exception? innerException) : base(message, innerException) {
		}
	}
}
