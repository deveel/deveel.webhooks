using System;

namespace Deveel.Webhooks {
	class WebhookFilterField : IWebhookFilter {
		public string Expression { get; set; }

		public string Provider { get; set; }

		public string Format { get; set; }
	}
}
