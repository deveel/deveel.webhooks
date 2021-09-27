using System;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

using Microsoft.Extensions.Options;

using Newtonsoft.Json.Linq;

namespace Deveel.Webhooks {
	public class DefaultHttptWebhookReceiver<T> : IHttpWebhookReceiver<T> where T : class {
		private readonly WebhookReceiveOptions options;
		private readonly Action<JObject, T> afterRead;

		public DefaultHttptWebhookReceiver(IOptions<WebhookReceiveOptions> options)
			: this(options, null) {
		}

		public DefaultHttptWebhookReceiver(IOptions<WebhookReceiveOptions> options, Action<JObject, T> afterRead)
			: this(options?.Value, afterRead) {
		}

		public DefaultHttptWebhookReceiver(WebhookReceiveOptions options, Action<JObject, T> afterRead = null) {
			this.options = options;
			this.afterRead = afterRead;
		}

		public DefaultHttptWebhookReceiver()
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
