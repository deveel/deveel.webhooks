using System;
using System.Collections.Generic;

using Deveel.Events;

namespace Deveel.Webhooks {
	public interface IWebhook : IEvent {
		string Name { get; }

		string SubscriptionId { get; }

		string DestinationUrl { get; }

		string Secret { get; }

		IDictionary<string, string> Headers { get; }
	}
}
