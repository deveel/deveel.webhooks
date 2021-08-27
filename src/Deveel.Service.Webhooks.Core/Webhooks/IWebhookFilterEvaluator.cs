using System;
using System.Threading;
using System.Threading.Tasks;

namespace Deveel.Webhooks {
	public interface IWebhookFilterEvaluator {
		Task<bool> MatchesAsync(object filter, IWebhook webhook, CancellationToken cancellationToken);
	}
}
