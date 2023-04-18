using System;

namespace Deveel.Webhooks {
	public class WebhookReceiverOptions<TWebhook> {
		public bool? VerifySignature { get; set; }

		public WebhookSignatureOptions Signature { get; set; } = new WebhookSignatureOptions();

		public int? ResponseStatusCode { get; set; } = 201;
	}
}
