using System;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

using Microsoft.Extensions.Options;

using Newtonsoft.Json.Linq;

namespace Deveel.Webhooks {
	public class DefaulHttptWebhookReceiver<T> : IHttpWebhookReceiver<T> where T : class {
		private readonly WebhookReceiveOptions options;
		private readonly Action<JObject, T> afterRead;

		public DefaulHttptWebhookReceiver(IOptions<WebhookReceiveOptions> options)
			: this(options, null) {
		}

		public DefaulHttptWebhookReceiver(IOptions<WebhookReceiveOptions> options, Action<JObject, T> afterRead)
			: this(options?.Value, afterRead) {
		}

		public DefaulHttptWebhookReceiver(WebhookReceiveOptions options, Action<JObject, T> afterRead = null) {
			this.options = options;
			this.afterRead = afterRead;
		}

		public DefaulHttptWebhookReceiver()
			: this(new WebhookReceiveOptions()) {
		}

		protected virtual void OnAfterRead(JObject json, T obj) {
			afterRead?.Invoke(json, obj);
		}

		public virtual Task<T> ReceiveAsync(HttpRequestMessage request, CancellationToken cancellationToken) {
			return request.GetWebhookAsync<T>(options, OnAfterRead, cancellationToken);
		}
	}
}
