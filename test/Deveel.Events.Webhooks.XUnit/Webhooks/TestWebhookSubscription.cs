using System;
using System.Collections.Generic;

namespace Deveel.Webhooks {
	public class TestWebhookSubscription : IWebhookSubscription {
		public string SubscriptionId { get; set; }

		public string TenantId { get; set; }

		public string Name { get; set; }

		public IEnumerable<string> EventTypes { get; set; }

		public string DestinationUrl { get; set; }

		public string Secret { get; set; }

		public WebhookSubscriptionStatus Status { get; set; }

		public int? RetryCount { get; set; }

		public IEnumerable<IWebhookFilter> Filters { get; set; }

		public IDictionary<string, string> Headers { get; set; }

		public IDictionary<string, object> Metadata { get; set; }

		public DateTimeOffset? CreatedAt { get; set; }

		public DateTimeOffset? UpdatedAt { get; set; }

		public string? Format { get; set; }
	}
}
