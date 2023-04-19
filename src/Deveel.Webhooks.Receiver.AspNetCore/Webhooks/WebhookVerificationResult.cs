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
    /// Represents a default implementation of a result 
	/// of the verification of a webhook request.
    /// </summary>
    public readonly struct WebhookVerificationResult : IWebhookVerificationResult {
        /// <summary>
        /// Constructs the result of a verification of a webhook request.
        /// </summary>
        /// <param name="isVerified">
        /// Whether the request is verified or not
        /// </param>
        private WebhookVerificationResult(bool isVerified) {
            IsVerified = isVerified;
        }

        /// <summary>
        /// Gets whether the request is verified or not.
        /// </summary>
        public bool IsVerified { get; }

        /// <summary>
        /// Creates a new result of a successful verification of a webhook request
        /// </summary>
        /// <returns>
        /// Returns an instance of <see cref="WebhookVerificationResult"/> that
        /// represents a successful verification of a webhook request.
        /// </returns>
        public static WebhookVerificationResult Verified { get; } = new WebhookVerificationResult(true);

        /// <summary>
        /// Creates a new result of a failed verification of a webhook request
        /// </summary>
        /// <returns>
        /// Returns a new instance of <see cref="WebhookVerificationResult"/> that
        /// represents a failed verification of a webhook request.
        /// </returns>
        public static WebhookVerificationResult Failed { get; } = new WebhookVerificationResult(false);
    }
}
