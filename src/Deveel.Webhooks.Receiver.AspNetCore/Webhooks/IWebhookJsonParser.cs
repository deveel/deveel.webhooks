using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace Deveel.Webhooks {
	public interface IWebhookJsonParser<TWebhook> {
		Task<TWebhook> ParseWebhookAsync(Stream utf8Stream, CancellationToken cancellationToken = default);
	}
}
