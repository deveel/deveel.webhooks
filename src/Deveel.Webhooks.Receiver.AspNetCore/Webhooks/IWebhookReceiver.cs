using System;
using System.Threading.Tasks;
using System.Threading;

using Microsoft.AspNetCore.Http;

namespace Deveel.Webhooks {
	public interface IWebhookReceiver<T> where T : class {
		Task<T> ReceiveAsync(HttpRequest request, CancellationToken cancellationToken);
	}
}
