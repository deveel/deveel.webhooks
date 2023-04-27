using Deveel.Facebook;

using Microsoft.Extensions.Options;

namespace Deveel.Webhooks {
	class ConfigureWebhookVerificationOptions : IPostConfigureOptions<WebhookVerificationOptions<FacebookWebhook>> {
		private readonly FacebookReceiverOptions receiverOptions;

		public ConfigureWebhookVerificationOptions(IOptions<FacebookReceiverOptions> receiverOptions) {
			this.receiverOptions = receiverOptions.Value;
		}

		public void PostConfigure(string name, WebhookVerificationOptions<FacebookWebhook> options) {
			options.SuccessStatusCode = 200;
			options.NotAuthenticatedStatusCode = 403;
			options.VerificationToken = receiverOptions.VerifyToken;
			options.VerificationTokenQueryName = "hub.verify_token";
			// TODO: options.ChallengeSender = true;
			// TODO: options.ChallengeQueryName = "hub.challenge";
		}
	}
}