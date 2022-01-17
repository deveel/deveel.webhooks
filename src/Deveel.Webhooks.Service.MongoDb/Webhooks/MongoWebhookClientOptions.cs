using System;

namespace Deveel.Webhooks {
	public class MongoWebhookClientOptions {
		public string Database { get; set; }

		public string ConnectionString { get; set; }

		public string SubscriptionCollection { get; set; }

		public string WebhookCollection { get; set; }

		public string SslProtocol { get; set; }
	}
}
