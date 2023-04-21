using System;
using System.Threading;
using System.Threading.Tasks;

namespace Deveel.Webhooks {
	public interface IWebhookFactory<TWebhook> where TWebhook : class {
		Task<TWebhook> CreateAsync(IWebhookSubscription subscription, EventInfo eventInfo, CancellationToken cancellationToken);
	}
}
