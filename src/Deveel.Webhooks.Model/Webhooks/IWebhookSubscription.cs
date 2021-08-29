using System;
using System.Collections.Generic;

namespace Deveel.Webhooks {
	public interface IWebhookSubscription {
		string SubscriptionId { get; }

		string Name { get; }

		string[] EventTypes { get; }

		string DestinationUrl { get; }

		string Secret { get; }

		bool IsActive { get; }

		int RetryCount { get; }

		IEnumerable<IWebhookFilter> Filters { get; }

		IDictionary<string, string> Headers { get; }

		IDictionary<string, object> Metadata { get; }
	}
}
