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
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Deveel.Webhooks {
	/// <summary>
	/// Represents a result of the verification of the receiver
	/// made by the Facebook.
	/// </summary>
	public readonly struct FacebookVerificationResult : IWebhookVerificationResult {
		internal FacebookVerificationResult(bool valid, bool isVerified, string? challenge = null) {
			IsValid = valid;
			IsVerified = isVerified;
			Challenge = challenge;
		}

		/// <inheritdoc/>
		public bool IsVerified { get; }

		/// <summary>
		/// Gets the challenge string to be sent back to Facebook
		/// </summary>
		public string? Challenge { get; }

		/// <inheritdoc/>
		public bool IsValid { get; }
	}
}
