using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Deveel.Webhooks {
	public sealed class WebhookDeliveryResult<TWebhook> where TWebhook : class {
		private readonly bool failed;
		private readonly List<WebhookDeliveryAttempt> attempts = new List<WebhookDeliveryAttempt>();

		public WebhookDeliveryResult(WebhookDestination destination, TWebhook webhook) : this(destination, webhook, false) {
		}

		private WebhookDeliveryResult(WebhookDestination destination, TWebhook webhook, bool failed) {
			Destination = destination ?? throw new ArgumentNullException(nameof(destination));
			Webhook = webhook ?? throw new ArgumentNullException(nameof(webhook));
			this.failed = failed;
		}

		public IEnumerable<WebhookDeliveryAttempt> Attempts {
			get {
				lock (attempts) {
					return attempts.AsReadOnly();
				}
			}
		}

		public WebhookDestination Destination { get; }

		public TWebhook Webhook { get; }

		public WebhookDeliveryAttempt LastAttempt => Attempts
			.OrderByDescending(x => x.Number).FirstOrDefault();

		public bool HasAttempted {
			get {
				lock (attempts) {
					return attempts.Count > 0;
				}
			}
		}

		public bool Successful => !failed && Attempts.All(x => !x.Failed);

		public int AttemptCount {
			get {
				lock (attempts) {
					return attempts.Count;
				}
			}
		}

		public WebhookDeliveryAttempt StartAttempt() {
			lock (attempts) {
				var attempt = WebhookDeliveryAttempt.Start(attempts.Count + 1);
				attempts.Add(attempt);

				return attempt;
			}
		}

		public static WebhookDeliveryResult<TWebhook> Fail(WebhookDestination destination, TWebhook webhook) 
			=> new WebhookDeliveryResult<TWebhook>(destination, webhook, true);
	}
}
