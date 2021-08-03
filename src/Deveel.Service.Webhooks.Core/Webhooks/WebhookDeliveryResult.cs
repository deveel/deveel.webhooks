using System;
using System.Collections.Generic;
using System.Linq;

namespace Deveel.Webhooks {
	public sealed class WebhookDeliveryResult {
		private readonly List<WebhookDeliveryAttempt> _attempts;

		public WebhookDeliveryResult(IWebhook webhook) {
			Webhook = webhook ?? throw new ArgumentNullException(nameof(webhook));
			_attempts = new List<WebhookDeliveryAttempt>();
		}

		public IWebhook Webhook { get; }

		public IEnumerable<WebhookDeliveryAttempt> Attempts => _attempts.AsReadOnly();

		public WebhookDeliveryAttempt LastAttempt => Attempts
			.Where(x => x.EndedAt != null)
			.OrderByDescending(x => x.EndedAt.Value).FirstOrDefault();

		public bool Successful => Attempts.All(x => !x.Failed);

		public WebhookDeliveryAttempt StartNew() {
			var attempt = new WebhookDeliveryAttempt();
			_attempts.Add(attempt);

			return attempt;
		}
	}
}
