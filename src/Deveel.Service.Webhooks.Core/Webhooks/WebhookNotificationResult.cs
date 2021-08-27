using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Deveel.Webhooks {
	public sealed class WebhookNotificationResult : IEnumerable<WebhookDeliveryResult> {
		private readonly List<WebhookDeliveryResult> deliveryResults;

		public WebhookNotificationResult() {
			deliveryResults = new List<WebhookDeliveryResult>();
		}

		public void AddDelivery(WebhookDeliveryResult result) {
			lock(this) {
				deliveryResults.Add(result);
			}
		}

		public bool HasSuccessful => Successful?.Any() ?? false;

		public IEnumerable<WebhookDeliveryResult> Successful
			=> deliveryResults.Where(x => x.Successful);

		public bool HasFailed => Failed?.Any() ?? false;

		public IEnumerable<WebhookDeliveryResult> Failed
			=> deliveryResults.Where(x => !x.Successful);

		public bool IsEmpty => deliveryResults.Count == 0;

		public WebhookDeliveryResult this[string subscriptionId] => deliveryResults.ToDictionary(x => x.Webhook.SubscriptionId, y => y)[subscriptionId];

		public IEnumerator<WebhookDeliveryResult> GetEnumerator() => deliveryResults.GetEnumerator();

		IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
	}
}
