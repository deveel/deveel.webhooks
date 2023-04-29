// Copyright 2022-2023 Deveel
// 
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
// 
//     http://www.apache.org/licenses/LICENSE-2.0
// 
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.

using Deveel.Webhooks.SendGrid;

using Microsoft.Extensions.Options;

namespace Deveel.Webhooks {
	class ConfigureWebhookReceiverOptions : IPostConfigureOptions<WebhookReceiverOptions<SendGridWebhook>> {
		private readonly SendGridReceiverOptions receiverOptions;

		public ConfigureWebhookReceiverOptions(IOptions<SendGridReceiverOptions> receiverOptions) {
			this.receiverOptions = receiverOptions.Value;
		}

		public void PostConfigure(string name, WebhookReceiverOptions<SendGridWebhook> options) {
			options.ContentFormats = WebhookContentFormats.Json;
			options.JsonParser = new SystemTextWebhookJsonParser<SendGridWebhook>(SendGridWebhookParser.Options);
			options.RootType = WebhookRootType.List;

			options.VerifySignature = receiverOptions.VerifySignature;
			options.Signature.Algorithm = "sha256";
			options.Signature.ParameterName = "X-Twilio-Email-Event-Webhook-Signature";
			options.Signature.Location = WebhookSignatureLocation.Header;
			options.Signature.Secret = receiverOptions.Secret;
			options.Signature.Signer = new Sha256WebhookSigner();
		}
	}
}
