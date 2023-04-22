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

namespace Deveel.Webhooks {
	/// <summary>
	/// PRovides a static factory to create a signature for a webhook
	/// </summary>
    public static class WebhookSignature {
		/// <summary>
		/// Creates a signature for the given <paramref name="algorithm"/>,
		/// the <paramref name="webhookBody"/> payload and the <paramref name="secret"/>
		/// provided by a subcription to sign the payload.
		/// </summary>
		/// <param name="algorithm">
		/// The name of the algorithm to use to sign the payload.
		/// </param>
		/// <param name="webhookBody">
		/// The body of the webhook payload to be signed.
		/// </param>
		/// <param name="secret">
		/// A secret provided by a subscription to sign the payload.
		/// </param>
		/// <returns>
		/// Returns a string representing the signature of the payload.
		/// </returns>
		/// <exception cref="NotSupportedException">
		/// Thrown when the <paramref name="algorithm"/> is not supported.
		/// </exception>
        public static string Create(string algorithm, string webhookBody, string secret) {
            IWebhookSigner signer;

            switch (algorithm.ToUpperInvariant()) {
                case "SHA1":
                case "SHA-1":
                    signer = new Sha1WebhookSigner();
                    break;
                case "SHA256":
                case "SHA-256":
                    signer = new Sha256WebhookSigner();
                    break;
                default:
                    throw new NotSupportedException($"The algorithm '{algorithm}' is not supported.");
            }

            return signer.SignWebhook(webhookBody, secret);
        }
    }
}
