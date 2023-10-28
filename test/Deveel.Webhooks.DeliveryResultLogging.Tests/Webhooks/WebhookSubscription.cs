namespace Deveel.Webhooks {
	public sealed class WebhookSubscription : IWebhookSubscription {
		public string? SubscriptionId { get; set; }

		public string? TenantId { get; set; }

		public string Name { get; set; }

		IEnumerable<string> IWebhookSubscription.EventTypes => EventTypes;

		public string[] EventTypes { get; set; }

		public string DestinationUrl { get; set; }

		public string? Secret { get; set; }

		public string? Format { get; set; }

		public WebhookSubscriptionStatus Status { get; set; }

		public int? RetryCount { get; set; }

		IEnumerable<IWebhookFilter>? IWebhookSubscription.Filters => Enumerable.Empty<IWebhookFilter>();

		public IDictionary<string, string>? Headers { get; set; } = new Dictionary<string, string>();

		public IDictionary<string, object>? Properties { get; set; } = new Dictionary<string, object>();

		public DateTimeOffset? CreatedAt { get; set; }

		public DateTimeOffset? UpdatedAt { get; set; }
	}
}
