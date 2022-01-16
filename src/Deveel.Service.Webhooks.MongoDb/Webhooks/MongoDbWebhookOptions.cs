using System;

namespace Deveel.Webhooks {
	public sealed class MongoDbWebhookOptions {
		public string DatabaseName { get; set; }

		public string SubscriptionsCollectionName { get; set; }

		public string WebhooksCollectionName { get; set; }

		public string ConnectionString { get; set; }

		public string TenantId { get; set; }

		public string TenantDatabaseFormat { get; set; } = "{tenant}_{database}";

		public string TenantCollectionFormat { get; set; } = "{tenant}_{collection}";

		public string TenantField { get; set; } = "TenantId";

		public MongoDbMultiTenancyHandling MultiTenantHandling { get; set; }
	}
}
