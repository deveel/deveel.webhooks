using System;

namespace Deveel.Webhooks {
	public class WebhookSenderOptions {
		public string? HttpClientName { get; set; }

		public IDictionary<string, string>? DefaultHeaders { get; set; } = new Dictionary<string, string>();

		public WebhookRetryOptions? Retry { get; set; } = new WebhookRetryOptions();

		public WebhookSenderSignatureOptions? Signature { get; set; } = new WebhookSenderSignatureOptions();

		public bool? VerifyReceivers { get; set; }
	}
}