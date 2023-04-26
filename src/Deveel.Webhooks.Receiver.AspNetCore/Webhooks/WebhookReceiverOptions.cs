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
	/// Provides the configuration options for a webhook receiver.
	/// </summary>
	public class WebhookReceiverOptions {
		/// <summary>
		/// Gets or sets whether the signature of the incoming webhook
		/// should be verified.
		/// </summary>
		public bool? VerifySignature { get; set; }

		/// <summary>
		/// Gets or sets the options for the signature verification.
		/// </summary>
		public WebhookSignatureOptions Signature { get; set; } = new WebhookSignatureOptions();
	}
}
