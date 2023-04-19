using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Deveel.Webhooks {
	public interface IWebhookJsonSerializer<TWebhook> where TWebhook : class {
		Task SerializeWebhookAsync(Stream utf8Stream, TWebhook webhook, CancellationToken cancellationToken);
	}
}
