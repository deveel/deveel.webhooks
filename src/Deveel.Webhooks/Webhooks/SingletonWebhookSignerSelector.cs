using System;

namespace Deveel.Webhooks {
	class SingletonWebhookSignerSelector : IWebhookSignerSelector {
		private readonly IWebhookSigner webhookSigner;

		public SingletonWebhookSignerSelector(IWebhookSigner webhookSigner) {
			this.webhookSigner = webhookSigner;
		}

		public IWebhookSigner GetSigner(string algorithm) {
			if (algorithm == webhookSigner.Algorithm)
				return webhookSigner;

			return null;
		}
	}
}
