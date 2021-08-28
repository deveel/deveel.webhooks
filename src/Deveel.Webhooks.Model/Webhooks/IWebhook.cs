using System;
using System.Collections.Generic;

namespace Deveel.Webhooks {
	public interface IWebhook {
		string Id { get; }

		string Name { get; }

		string SubscriptionId { get; }

		string DestinationUrl { get; }

		string Secret { get; }

		IDictionary<string, string> Headers { get; }

		DateTimeOffset TimeStamp { get; }

		string EventType { get; }

		object Data { get; }
	}
}
