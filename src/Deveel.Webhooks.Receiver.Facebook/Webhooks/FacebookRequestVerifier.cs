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
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;

namespace Deveel.Webhooks
{
    class FacebookRequestVerifier : WebhookRequestVerifier<FacebookWebhook> {
		public FacebookRequestVerifier(IOptions<WebhookVerificationOptions<FacebookWebhook>> options) 
			: base(options) {
		}

		private const string ChallengeQueryName = "hub.challenge";

		protected override async Task OnSuccessAsync(IWebhookVerificationResult result, HttpResponse httpResponse, CancellationToken cancellationToken) {
			var fbResult = (FacebookVerificationResult)result;
			httpResponse.ContentType = "text/plain";
			httpResponse.StatusCode = 200;
			await httpResponse.WriteAsync(fbResult.Challenge);
		}

		public override async Task<IWebhookVerificationResult> VerifyRequestAsync(HttpRequest httpRequest, CancellationToken cancellationToken = default) {
			await Task.CompletedTask;

			if (!TryGetVerificationToken(httpRequest, out var verifyTyoken))
				return new FacebookVerificationResult(false, false);

			if (!TryGetChallenge(httpRequest, out var challenge)) 
				return new FacebookVerificationResult(false, false);

			if (!String.Equals(verifyTyoken, VerificationOptions.VerificationToken))
				return new FacebookVerificationResult(false, true);

			return new FacebookVerificationResult(true, true, challenge);
		}

		private bool TryGetChallenge(HttpRequest httpRequest, out string? challenge) {
			if (!httpRequest.Query.TryGetValue(ChallengeQueryName, out var challengeValues)) {
				challenge = null;
				return false;
			}

			challenge = challengeValues.FirstOrDefault();
			return true;
		}
	}
}
