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

namespace Deveel.Webhooks {
    /// <summary>
    /// Represents a default implementation of a result 
	/// of the verification of a webhook request.
    /// </summary>
    public readonly struct WebhookVerificationResult : IWebhookVerificationResult {
		/// <summary>
		/// Constructs the result of a verification of a webhook request.
		/// </summary>
		/// <param name="valid">
		/// Indicates if the request is valid or not
		/// </param>
		/// <param name="isVerified">
		/// Whether the request is verified or not
		/// </param>
		private WebhookVerificationResult(bool valid, bool isVerified) {
			IsValid = valid;
            IsVerified = isVerified;
        }

        /// <summary>
        /// Gets whether the request is verified or not.
        /// </summary>
        public bool IsVerified { get; }

		/// <summary>
		/// Gets whether the request is valid or not.
		/// </summary>
		public bool IsValid { get; }

        /// <summary>
        /// A result of a successful verification of a webhook request
        /// </summary>
        public static WebhookVerificationResult Verified { get; } = new WebhookVerificationResult(true, true);

        /// <summary>
        /// A result that indicates a failed verification of a webhook request
        /// </summary>
        public static WebhookVerificationResult NotVerified { get; } = new WebhookVerificationResult(true, false);

		/// <summary>
		/// A result that indicated  an invalid verification request (eg. missing parameters)
		/// </summary>
		public static WebhookVerificationResult Invalid { get; } = new WebhookVerificationResult(false, false);
    }
}
