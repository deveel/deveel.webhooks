using System.Threading;
using System.Threading.Tasks;

namespace Deveel.Webhooks {
	public interface IWebhookHandler<TWebhook> where TWebhook : class {
		Task HandleAsync(TWebhook webhook, CancellationToken cancellationToken = default);
	}
}
