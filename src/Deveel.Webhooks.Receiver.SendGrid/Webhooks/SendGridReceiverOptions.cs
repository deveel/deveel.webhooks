// Copyright 2022-2024 Antonello Provenzano
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
	/// The options used to configure the infrastructure to receive
	/// webhooks from SendGrid.
	/// </summary>
	public sealed class SendGridReceiverOptions {
		/// <summary>
		/// Gets or sets the secret used to verify the signature of the
		/// webhooks received from SendGrid.
		/// </summary>
		public string? Secret { get; set; }

		/// <summary>
		/// Gets or sets a value indicating if the signature of the webhook
		/// sent by SendGrid should be verified.
		/// </summary>
		public bool? VerifySignature { get; set; }
	}
}
