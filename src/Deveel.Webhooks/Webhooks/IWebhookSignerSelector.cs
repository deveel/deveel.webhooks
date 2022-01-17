using System;

namespace Deveel.Webhooks {
	public interface IWebhookSignerSelector {
		IWebhookSigner GetSigner(string algorithm);
	}
}
