using System;

namespace Deveel.Webhooks {
	public class MongoWebhookRepositoryOptions {
		public string Database { get; set; }

		public string ConnectionString { get; set; }

		public string Collection { get; set; }

		public string SslProtocol { get; set; }

		public string TenantId { get; set; }

		public bool HasTenant => !String.IsNullOrWhiteSpace(TenantId);
	}
}
