using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Deveel.Webhooks {
	interface IWebhookHandlerInitialize<TWebhook> : IWebhookHandler<TWebhook> where TWebhook : class {
		void Initialize(IServiceProvider provider);
	}
}
