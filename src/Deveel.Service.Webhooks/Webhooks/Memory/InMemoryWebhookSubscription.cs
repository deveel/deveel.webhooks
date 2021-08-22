using System;
using System.Collections.Generic;

namespace Deveel.Webhooks.Memory {
	public class InMemoryWebhookSubscription : IWebhookSubscription {
		public string Name { get; set; }

		public string DestinationUrl { get; set; }

		public string Secret { get; set; }

		public bool IsActive { get; set; }

		public int RetryCount { get; set; }

		public IDictionary<string, string> Headers { get; set; }

		public IDictionary<string, object> Metadata { get; set; }

		public string EventType { get; set; }

		public IEnumerable<string> FilterExpressions { get; set; }

		public string Id { get; set; }
	}
}
