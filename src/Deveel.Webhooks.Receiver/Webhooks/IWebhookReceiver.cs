using System;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace Deveel.Webhooks {
	public interface IWebhookReceiver<T> where T : class {
		Task<T> ReceiveAsync(HttpRequestMessage request, CancellationToken cancellationToken);
	}
}
