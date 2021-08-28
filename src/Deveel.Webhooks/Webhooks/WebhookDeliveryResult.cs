using System;
using System.Collections.Generic;
using System.Linq;

namespace Deveel.Webhooks {
	public sealed class WebhookDeliveryResult {
		private readonly List<WebhookDeliveryAttempt> _attempts;
		private readonly bool failed;

		private WebhookDeliveryResult(IWebhook webhook, bool failed) {
			Webhook = webhook ?? throw new ArgumentNullException(nameof(webhook));
			this.failed = failed;
			_attempts = new List<WebhookDeliveryAttempt>();
		}

		public WebhookDeliveryResult(IWebhook webhook)
			: this(webhook, false) {
		}

		public IWebhook Webhook { get; }

		public IEnumerable<WebhookDeliveryAttempt> Attempts => _attempts.AsReadOnly();

		public static WebhookDeliveryResult Fail(IWebhook webhook) => new WebhookDeliveryResult(webhook, true);

		public WebhookDeliveryAttempt LastAttempt => Attempts
			.Where(x => x.EndedAt != null)
			.OrderByDescending(x => x.EndedAt.Value).FirstOrDefault();

		public bool HasAttempted => _attempts.Count > 0;

		public bool Successful => !failed && Attempts.All(x => !x.Failed);

		public void AddAttempt(WebhookDeliveryAttempt attempt) {
			lock(this) {
				_attempts.Add(attempt);
			}
		}
	}
}
