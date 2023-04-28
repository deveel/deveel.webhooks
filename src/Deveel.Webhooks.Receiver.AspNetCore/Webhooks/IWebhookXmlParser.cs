using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Deveel.Webhooks {
	public interface IWebhookXmlParser<TWebhook> where TWebhook : class {
		Task<TWebhook> ParseWebhookAsync(Stream utf8Stream, CancellationToken cancellationToken = default);
	}
}
