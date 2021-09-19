using System;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

using Microsoft.AspNetCore.Http;

using Newtonsoft.Json.Linq;

namespace Deveel.Webhooks {
	public class DefaultWebhookReceiver<T> : IWebhookReceiver<T> where T : class {
		private readonly WebhookReceiveOptions options;
		private readonly Action<JObject, T> afterRead;

		protected virtual void OnAfterRead(JObject json, T obj) {
			afterRead?.Invoke(json, obj);
		}

		public Task<T> ReceiveAsync(HttpRequest request, CancellationToken cancellationToken) {
			return request.GetWebhookAsync<T>(options, OnAfterRead, cancellationToken);
		}
	}
}
