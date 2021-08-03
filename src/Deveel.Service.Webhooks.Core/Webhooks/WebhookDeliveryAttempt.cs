using System;

namespace Deveel.Webhooks {
	public sealed class WebhookDeliveryAttempt {
		internal WebhookDeliveryAttempt() {
			StartedAt = DateTimeOffset.UtcNow;
		}

		public bool HasResponse => ResponseStatusCode != null;

		public int? ResponseStatusCode { get; private set; }

		public string ResponseMessage { get; private set; }

		public DateTimeOffset StartedAt { get; }

		public DateTimeOffset? EndedAt { get; private set; }

		public bool Failed => ResponseStatusCode >= 400 || HasTimedOut;

		public bool HasTimedOut { get; private set; }

		public void Finish(int statusCode, string message) {
			ResponseStatusCode = statusCode;
			ResponseMessage = message;
			EndedAt = DateTimeOffset.UtcNow;
		}

		public void Timeout() {
			HasTimedOut = true;
			EndedAt = DateTimeOffset.UtcNow;
		}
	}
}
