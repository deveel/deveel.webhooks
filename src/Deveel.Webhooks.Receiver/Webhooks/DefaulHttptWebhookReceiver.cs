using System;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

using Microsoft.Extensions.Options;

using Newtonsoft.Json.Linq;

namespace Deveel.Webhooks {
	public class DefaulHttptWebhookReceiver<T> : IHttpWebhookReceiver<T> where T : class {
		private readonly Action<JObject, T> afterRead;

		public DefaulHttptWebhookReceiver(IOptions<WebhookReceiveOptions> options, Action<JObject, T> afterRead)
			: this(options?.Value, afterRead) {
		}

		public DefaulHttptWebhookReceiver(IOptions<WebhookReceiveOptions> options)
			: this(options, null) {
		}

		public DefaulHttptWebhookReceiver(WebhookReceiveOptions options, Action<JObject, T> afterRead = null) {
			Options = options ?? throw new ArgumentNullException(nameof(options));
			this.afterRead = afterRead;
		}

		public DefaulHttptWebhookReceiver()
			: this(new WebhookReceiveOptions()) {
		}

		protected virtual void OnAfterRead(JObject json, T obj) {
			afterRead?.Invoke(json, obj);
		}

		protected WebhookReceiveOptions Options { get; }

		public virtual Task<T> ReceiveAsync(HttpRequestMessage request, CancellationToken cancellationToken) {
			return request.GetWebhookAsync<T>(Options, OnAfterRead, cancellationToken);
		}
	}
}
