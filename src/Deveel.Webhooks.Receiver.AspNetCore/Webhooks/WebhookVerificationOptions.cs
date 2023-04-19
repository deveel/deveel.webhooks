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
	/// Provides the configuration options for the default verification 
	/// of a webhook request.
	/// </summary>
	public class WebhookVerificationOptions {
		/// <summary>
		/// Gets or sets a token that is matched against the value
		/// sent by the provider to verify the identity of the receiver.
		/// </summary>
		public string? VerificationToken { get; set; }

		/// <summary>
		/// Gets or sets the name of the query parameter that contains
		/// the verification token.
		/// </summary>
		public string? VerificationTokenQueryName { get; set; }

		/// <summary>
		/// Gets or sets the HTTP status code to return when the request
		/// is successfully verified (<c>204</c> by default).
		/// </summary>
		public int? SuccessStatusCode { get; set; } = 204;

		/// <summary>
		/// Gets or sets the HTTP status code to return when the request
		/// is not authenticated (<c>403</c> by default).
		/// </summary>
		public int? NotAuthenticatedStatusCode { get; set; } = 403;
	}
}
