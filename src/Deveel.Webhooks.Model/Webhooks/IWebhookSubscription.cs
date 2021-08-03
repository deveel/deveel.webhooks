using System;
using System.Collections.Generic;

using Deveel.Events;

namespace Deveel.Webhooks {
	public interface IWebhookSubscription : IEventSubscription {
		string Name { get; }

		string DestinationUrl { get; }

		string Secret { get; }

		bool IsActive { get; }

		int RetryCount { get; }

		IDictionary<string, string> Headers { get; }

		IDictionary<string, object> Metadata { get; }
	}
}
