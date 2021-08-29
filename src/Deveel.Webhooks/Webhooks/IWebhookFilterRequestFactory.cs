using System;

namespace Deveel.Webhooks {
	public interface IWebhookFilterRequestFactory {
		public WebhookFilterRequest CreateRequest(IWebhookSubscription subscription);
	}
}
