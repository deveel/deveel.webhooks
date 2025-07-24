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

using Microsoft.AspNetCore.Http;

namespace Deveel.Webhooks {
	/// <summary>
	/// A service that is used to verify a request of acknowledgement 
	/// by the sender of a webhook, before the webhook is sent.
	/// </summary>
	/// <typeparam name="TWebhook">
	/// The type of webhook that is being verified
	/// </typeparam>
	/// <remarks>
	/// <para>
	/// In several case scenarios, providers of webhooks require a verification
	/// of the party to ensure they are the ones who should be receiving the
	/// webhooks, and not a malicious party.
	/// </para>
	/// <para>
	/// The process of verification is usually a two-step process, where the
	/// request is first validated, and then the instance of the result of the 
	/// validation is passed back to the verifier to handle the result towards 
	/// the sender: this mechanism is used to ensure that the verification process
	/// is specific for the provider of the webhook, since the different methodologies
	/// implemented by various providers.
	/// </para>
	/// </remarks>
	public interface IWebhookRequestVerifier<TWebhook> {
        /// <summary>
        /// Verifies the request of acknowledgement of a webhook.
        /// </summary>
        /// <param name="httpRequest">
        /// The HTTP request that is carrying the information
        /// to acknowledge the webhook.
        /// </param>
        /// <param name="cancellationToken">
        /// A token that can be used to cancel the operation
        /// </param>
        /// <returns>
        /// Returns a <see cref="WebhookVerificationResult"/> that indicates the result
        /// of the verification operation.
        /// </returns>
		Task<IWebhookVerificationResult> VerifyRequestAsync(HttpRequest httpRequest, CancellationToken cancellationToken = default);


		/// <summary>
		/// Handles the result of the verification of a webhook request.
		/// </summary>
		/// <param name="result">The result of the verification that should be handled</param>
		/// <param name="httpResponse">The HTTP response used to notify the sender</param>
		/// <param name="cancellationToken">
		/// A token that can be used to cancel the operation
		/// </param>
		/// <returns>
		/// Returns a <see cref="Task"/> that completes when the result is handled.
		/// </returns>
		Task HandleResultAsync(IWebhookVerificationResult result, HttpResponse httpResponse, CancellationToken cancellationToken);
	}
}
