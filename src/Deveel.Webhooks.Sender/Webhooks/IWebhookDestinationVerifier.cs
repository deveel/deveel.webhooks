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

namespace Deveel.Webhooks {
	/// <summary>
	/// A service that verifies the destination of a webhook, before
	/// attempting the delivery.
	/// </summary>
	/// <typeparam name="TWebhook">
	/// The type of the webhook that is delivered, used to
	/// segregate the verification service to a given scope.
	/// </typeparam>
	public interface IWebhookDestinationVerifier<TWebhook> where TWebhook : class {
        /// <summary>
        /// Verifies that the given destination is valid and reachable.
        /// </summary>
		/// <param name="destination">
		/// The object that describes the destination of a webhook and the
		/// configurations to be used by the verifier.
		/// </param>
        /// <param name="cancellationToken">
		/// A cancellation token used to cancel the operation.
		/// </param>
        /// <returns>
        /// Returns <c>true</c> if the destination is valid and reachable,
        /// otherwise it returns <c>false</c>.
        /// </returns>
        Task<DestinationVerificationResult> VerifyDestinationAsync(WebhookDestination destination, CancellationToken cancellationToken = default);
    }
}
