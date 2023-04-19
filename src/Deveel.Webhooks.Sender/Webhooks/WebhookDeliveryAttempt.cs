using System.Net;

namespace Deveel.Webhooks {
	public struct WebhookDeliveryAttempt {
		private WebhookDeliveryAttempt(int number, DateTimeOffset startedAt) : this() {
			Number = number;
			StartedAt = startedAt;
		}

		public int Number { get; }

		public int? ResponseCode { get; private set; }

		public string? ResponseMessage { get; private set; }

		public DateTimeOffset StartedAt { get; }

		public DateTimeOffset? CompletedAt { get; private set; }

		public bool HasCompleted => CompletedAt != null;

		public bool HasResponse => ResponseCode != null;

		public bool Failed => (HasCompleted && ResponseCode == null) || (ResponseCode >= 400);

		public TimeSpan? Elapsed => CompletedAt?.Subtract(StartedAt);

		public static WebhookDeliveryAttempt Start(int number) {
			if (number < 1)
				throw new ArgumentOutOfRangeException(nameof(number), "The number of the attempt must be greater than zero.");

			return new WebhookDeliveryAttempt(number, DateTimeOffset.UtcNow);
		}

		public void Complete(int responseCode, string? responseMessage) {
			ResponseCode = responseCode;
			ResponseMessage = responseMessage;
			CompletedAt = DateTimeOffset.UtcNow;
		}

		public void LocalFail(string? responseMessage) {
			ResponseCode = null;
			ResponseMessage = responseMessage;
			CompletedAt = DateTimeOffset.UtcNow;
		}

		public void TimeOut() => Complete((int)HttpStatusCode.RequestTimeout, "Request Timeout");
	}
}
