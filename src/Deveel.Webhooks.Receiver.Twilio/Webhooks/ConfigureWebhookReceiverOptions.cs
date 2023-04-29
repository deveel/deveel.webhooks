using System.Security.Cryptography;
using System.Text;

using Deveel.Webhooks.Twilio;

using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;

namespace Deveel.Webhooks {
	class ConfigureWebhookReceiverOptions : IPostConfigureOptions<WebhookReceiverOptions<TwilioWebhook>> {
		private readonly TwilioReceiverOptions twilioOptions;

		public ConfigureWebhookReceiverOptions(IOptions<TwilioReceiverOptions> twilioOptions) {
			this.twilioOptions = twilioOptions.Value;
		}

		public void PostConfigure(string name, WebhookReceiverOptions<TwilioWebhook> options) {
			options.ContentFormats = WebhookContentFormats.Form;
			options.FormParser = new TwilioWebhookFormParser();
			options.VerifySignature = twilioOptions.VerifySignature;
			options.Signature.Algorithm = "sha1";
			options.Signature.Location = WebhookSignatureLocation.Header;
			options.Signature.ParameterName = "X-Twilio-Signature";
			options.Signature.Secret = twilioOptions.AuthToken;
			options.Signature.OnCreate = request => CreateSignature(request, options.Signature.Secret);
		}

		private Task<string> CreateSignature(HttpRequest request, string? authToken) {
			if (String.IsNullOrWhiteSpace(authToken))
				return Task.FromResult(String.Empty);

			var result = TwilioSignature.Create(request, authToken);
			return Task.FromResult(result);
		}
	}
}
