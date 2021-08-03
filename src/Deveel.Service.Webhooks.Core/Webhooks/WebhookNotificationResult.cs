using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Deveel.Webhooks {
	public sealed class WebhookNotificationResult : IEnumerable<KeyValuePair<string, WebhookDeliveryResult>> {
		public WebhookNotificationResult() {
			SubscriptionResults = new Dictionary<string, WebhookDeliveryResult>();
		}

		public WebhookNotificationResult(IDictionary<string, WebhookDeliveryResult> source) {
			SubscriptionResults = new Dictionary<string, WebhookDeliveryResult>(source);
		}

		public IDictionary<string, WebhookDeliveryResult> SubscriptionResults { get; set; }

		public bool HasSuccessful => Successful?.Any() ?? false;

		public IEnumerable<KeyValuePair<string, WebhookDeliveryResult>> Successful
			=> SubscriptionResults?.Where(x => x.Value.Successful);

		public bool HasFailed => Failed?.Any() ?? false;

		public IEnumerable<KeyValuePair<string, WebhookDeliveryResult>> Failed
			=> SubscriptionResults?.Where(x => !x.Value.Successful);

		public bool IsEmpty => SubscriptionResults?.Count == 0;

		public IEnumerable<string> SubscriptionIds => SubscriptionResults?.Keys;

		public WebhookDeliveryResult this[string subscriptionId] => SubscriptionResults[subscriptionId];

		public IEnumerator<KeyValuePair<string, WebhookDeliveryResult>> GetEnumerator() => SubscriptionResults?.GetEnumerator();

		IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
	}
}
