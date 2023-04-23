using System;

namespace Deveel.Webhooks {
	public interface IWebhookReceiver {
		string SubscriptionId { get; }

		string SubscriptionName { get; }

		string DestinationUrl { get; }

		IEnumerable<KeyValuePair<string, string>> Headers { get; }

		string BodyFormat { get; }
	}
}
