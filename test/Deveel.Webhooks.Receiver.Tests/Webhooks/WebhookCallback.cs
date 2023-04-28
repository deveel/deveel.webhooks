using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Deveel.Webhooks {
	public static class WebhookCallback {
		public static IWebhookCallback<TWebhook> Create<TWebhook>(Action<TWebhook?> callback)
			where TWebhook : class {
			return new DelegatedWebhookCallback<TWebhook>(callback);
		}
	}
}
