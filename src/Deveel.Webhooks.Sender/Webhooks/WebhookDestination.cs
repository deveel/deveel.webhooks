using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Deveel.Webhooks {
	public sealed class WebhookDestination {
		public WebhookDestination(Uri url) {
			if (url == null)
				throw new ArgumentNullException(nameof(url));

			if (!url.IsAbsoluteUri)
				throw new ArgumentException($"The '{nameof(url)}' must be an absolute URI.", nameof(url));

			Url = url;
		}

		public WebhookDestination(string url)
			: this(ParseUri(url)) {
		}

		private static Uri ParseUri(string url) {
			if (!Uri.TryCreate(url, UriKind.Absolute, out var uri))
				throw new ArgumentException($"The '{nameof(url)}' must be an absolute URI.", nameof(url));

			return uri;
		}

		public Uri Url { get; }

		public bool? Verify { get; set; }

		public string? Secret { get; set; }

		public IDictionary<string, string>? Headers { get; set; } = new Dictionary<string, string>();

		public bool? Sign { get; set; }

		public WebhookSenderSignatureOptions? Signature { get; set; } = new WebhookSenderSignatureOptions();

		public WebhookRetryOptions? Retry { get; set; } = new WebhookRetryOptions();
	}
}
