using System;

namespace Deveel.Webhooks {
	public interface IWebhookFilterProviderRegistry {
		IWebhookFilterProvider GetProvider(string name);
	}
}
