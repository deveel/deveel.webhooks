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

using System;
using Deveel.Webhooks.Facebook;
using Microsoft.Extensions.Options;

namespace Deveel.Webhooks
{
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