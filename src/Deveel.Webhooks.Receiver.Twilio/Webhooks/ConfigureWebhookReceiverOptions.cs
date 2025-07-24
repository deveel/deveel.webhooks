// Copyright 2022-2025 Antonello Provenzano
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

		public void PostConfigure(string? name, WebhookReceiverOptions<TwilioWebhook> options) {
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
