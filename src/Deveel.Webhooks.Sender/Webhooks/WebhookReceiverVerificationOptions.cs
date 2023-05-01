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
	/// Provides options for the verification of the webhook destination.
	/// </summary>
    public class WebhookReceiverVerificationOptions {      
		/// <summary>
		/// Gets or sets the timeout for the verification request.
		/// </summary>
        public TimeSpan? Timeout { get; set; }

		/// <summary>
		/// Gets or sets the HTTP method to use for the verification request.
		/// </summary>
        public string HttpMethod { get; set; } = WebhookSenderDefaults.VerifyHttpMethod;

		/// <summary>
		/// Gets or sets the query parameter name to use for passing the
		/// token specific for the receiver in a verification request.
		/// </summary>
        public string TokenQueryParameter { get; set; } = WebhookSenderDefaults.VerifyTokenQueryParameterName;

		/// <summary>
		/// Gets or sets the header name to use for passing the
		/// token specific for the receiver in a verification request.
		/// </summary>
        public string TokenHeaderName { get; set; } = WebhookSenderDefaults.VerifyTokenHeaderName;

		/// <summary>
		/// Gets or sets a value indicating whether the verification request
		/// should be sent with a challenge response (default: <c>true</c>).
		/// </summary>
		public bool? Challenge { get; set; } = WebhookSenderDefaults.VerificationChallenge;

		/// <summary>
		/// Gets or sets the query parameter name to use for passing the
		/// challenge to the receiver in a verification request.
		/// </summary>
		public string? ChallengeQueryParameter { get; set; } = WebhookSenderDefaults.ChallengeQueryParameterName;

		/// <summary>
		/// Gets or sets the size of the challenge to be sent 
		/// in the verification request.
		/// </summary>
		public int? ChallengeLength { get; set; } = 32;

		/// <summary>
		/// Gets or sets the location of the token in the verification request.
		/// </summary>
		public VerificationTokenLocation TokenLocation { get; set; } = WebhookSenderDefaults.DefaultVerificationTokenLocation;
	}
}
