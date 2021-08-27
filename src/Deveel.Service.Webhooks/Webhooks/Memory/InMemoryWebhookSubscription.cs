using System;
using System.Collections.Generic;

namespace Deveel.Webhooks.Memory {
	public class InMemoryWebhookSubscription : IWebhookSubscription, IEntity {
		public string Name { get; set; }

		public string DestinationUrl { get; set; }

		public string Secret { get; set; }

		public bool IsActive { get; set; }

		public int RetryCount { get; set; }

		public IDictionary<string, string> Headers { get; set; }

		public IDictionary<string, object> Metadata { get; set; }

		public string[] EventTypes { get; set; }

		public IList<string> Filters { get; set; }

		object IWebhookSubscription.Filter => Filters;

		public string Id { get; set; }
	}
}
