﻿// Copyright 2022-2025 Antonello Provenzano
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
	/// Represents the result of a verification of a webhook request.
	/// </summary>
	/// <remarks>
	/// This contract is used by implementations of <see cref="IWebhookRequestVerifier{TWebhook}"/>
	/// to proceed with a two-step verification of a webhook request.
	/// </remarks>
	public interface IWebhookVerificationResult {
		/// <summary>
		/// Gets whether the request is verified or not.
		/// </summary>
		public bool IsVerified { get; }

		/// <summary>
		/// Gets a value indicating whether the request is valid or not.
		/// </summary>
		public bool IsValid { get; }
	}
}
