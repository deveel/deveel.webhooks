using System;
using System.Diagnostics;

namespace Deveel.Webhooks {
	public sealed class WebhookDeliveryAttempt {
		private readonly Stopwatch stopwatch;

		public WebhookDeliveryAttempt() {
			stopwatch = new Stopwatch();
			stopwatch.Start();
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
			stopwatch.Stop();

			ResponseStatusCode = statusCode;
			ResponseMessage = message;
			EndedAt = StartedAt.Add(stopwatch.Elapsed);
		}

		public void Timeout() {
			stopwatch.Stop();

			HasTimedOut = true;
			EndedAt = StartedAt.Add(stopwatch.Elapsed);
		}
	}
}
