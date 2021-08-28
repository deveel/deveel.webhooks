using System;
using System.Threading;
using System.Threading.Tasks;

namespace Deveel.Webhooks {
	public interface IWebhookFilterEvaluator {
		Task<bool> MatchesAsync(WebhookFilterRequest filterRequest, IWebhook webhook, CancellationToken cancellationToken);
	}
}
