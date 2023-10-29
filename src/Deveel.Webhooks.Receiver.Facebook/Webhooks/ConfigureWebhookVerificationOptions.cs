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

using Deveel.Webhooks.Facebook;

using Microsoft.Extensions.Options;

namespace Deveel.Webhooks {
	class ConfigureWebhookVerificationOptions : IPostConfigureOptions<WebhookVerificationOptions<FacebookWebhook>> {
		private readonly FacebookReceiverOptions receiverOptions;

		public ConfigureWebhookVerificationOptions(IOptions<FacebookReceiverOptions> receiverOptions) {
			this.receiverOptions = receiverOptions.Value;
		}

		public void PostConfigure(string? name, WebhookVerificationOptions<FacebookWebhook> options) {
			options.SuccessStatusCode = 200;
			options.NotAuthenticatedStatusCode = 403;
			options.VerificationToken = receiverOptions.VerifyToken;
			options.VerificationTokenQueryName = "hub.verify_token";
		}
	}
}