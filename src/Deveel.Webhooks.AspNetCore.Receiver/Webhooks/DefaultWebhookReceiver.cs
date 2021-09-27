using System;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;

using Newtonsoft.Json.Linq;

namespace Deveel.Webhooks {
	public class DefaultWebhookReceiver<T> : IWebhookReceiver<T> where T : class {
		private readonly Action<JObject, T> afterRead;

		public DefaultWebhookReceiver(IOptions<WebhookReceiveOptions> options, Action<JObject, T> afterRead)
			: this(options?.Value, afterRead) {
		}

		public DefaultWebhookReceiver(IOptions<WebhookReceiveOptions> options)
			: this(options, null) {
		}

		public DefaultWebhookReceiver(WebhookReceiveOptions options, Action<JObject, T> afterRead) {
			Options = options;
			this.afterRead = afterRead;
		}

		public DefaultWebhookReceiver(WebhookReceiveOptions options)
			: this(options, null) {
		}

		public DefaultWebhookReceiver()
			: this(new WebhookReceiveOptions()) {
		}

		protected WebhookReceiveOptions Options { get; }

		protected virtual void OnAfterRead(JObject json, T obj) {
			afterRead?.Invoke(json, obj);
		}

		public Task<T> ReceiveAsync(HttpRequest request, CancellationToken cancellationToken) {
			return request.GetWebhookAsync<T>(Options, OnAfterRead, cancellationToken);
		}
	}
}
