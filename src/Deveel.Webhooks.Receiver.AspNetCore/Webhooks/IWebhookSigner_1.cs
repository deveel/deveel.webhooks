using System;

namespace Deveel.Webhooks {
	public interface IWebhookSigner<TWebhook> : IWebhookSigner where TWebhook : class {
	}
}
