using System;

namespace Deveel.Webhooks.Storage {
	public interface IWebhookStore<TWebhook> where TWebhook : class, IWebhook {
	}
}
