using System;

namespace Deveel.Webhooks {
	public class MongoDbWebhookDeliveryAttempt : IWebhookDeliveryAttempt {
		public int? ResponseStatusCode { get; set; }

		public string ResponseMessage { get; set; }

		public DateTimeOffset StartedAt { get; set; }

		public DateTimeOffset? EndedAt { get; set; }
	}
}
