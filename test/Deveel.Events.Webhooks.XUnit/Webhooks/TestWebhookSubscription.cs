#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.


using System;
using System.Collections.Generic;
using System.Linq;

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

		IEnumerable<IWebhookFilter> IWebhookSubscription.Filters => Filters.Cast<IWebhookFilter>();

		public IList<WebhookFilter> Filters { get; set; }

		public IDictionary<string, string> Headers { get; set; }

		public IDictionary<string, object> Properties { get; set; }

		public DateTimeOffset? CreatedAt { get; set; }

		public DateTimeOffset? UpdatedAt { get; set; }

		public string? Format { get; set; }
	}
}
