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
    /// Defines a set of options for a Facebook Webhook receiver.
    /// </summary>
    public sealed class FacebookReceiverOptions {
		/// <summary>
		/// Gets or sets the application secret that is
		/// provided by Facebook to verify the signature
		/// of the incoming webhooks.
		/// </summary>
		public string? AppSecret { get; set; }

		/// <summary>
		/// Gets or sets whether the signature of the incoming webhook
		/// should be verified by the receiver.
		/// </summary>
		public bool? VerifySignature { get; set; }

		/// <summary>
		/// Gets or sets a token that was provided by Facebook
		/// to verify the receiving endpoint, before any
		/// webhook is delivered.
		/// </summary>
		public string? VerifyToken { get; set; }
	}
}
