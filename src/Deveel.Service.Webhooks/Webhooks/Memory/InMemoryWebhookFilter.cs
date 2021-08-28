using System;

namespace Deveel.Webhooks.Memory {
	public sealed class InMemoryWebhookFilter : IWebhookFilter {
		public string Expression { get; set; }

		public string Provider { get; set; }

		public string ExpressionFormat { get; set; }
	}
}
