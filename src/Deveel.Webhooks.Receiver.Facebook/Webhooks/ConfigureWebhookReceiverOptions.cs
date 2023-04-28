using System;

using Deveel.Facebook;

using Microsoft.Extensions.Options;

namespace Deveel.Webhooks {
	class ConfigureWebhookReceiverOptions : IPostConfigureOptions<WebhookReceiverOptions<FacebookWebhook>> {
		private readonly FacebookReceiverOptions facebookOptions;

		public ConfigureWebhookReceiverOptions(IOptions<FacebookReceiverOptions> facebookOptions) {
			this.facebookOptions = facebookOptions.Value;
		}

		public void PostConfigure(string name, WebhookReceiverOptions<FacebookWebhook> options) {
			options.Signature.AlgorithmHeaderName = "X-Hub-Signature";
			options.Signature.Algorithm = "sha256";
			options.Signature.Secret = facebookOptions.AppSecret;
			options.Signature.Signer = new Sha256WebhookSigner();
			options.VerifySignature = facebookOptions.VerifySignature;
			options.JsonParser = new SystemTextWebhookJsonParser<FacebookWebhook>(FacebookJsonSerializer.Options);
		}
	}
}