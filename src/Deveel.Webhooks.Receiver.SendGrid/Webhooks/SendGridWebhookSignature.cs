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
	/// Provides a helper to create a signature for a SendGrid webhook
	/// </summary>
	public static class SendGridWebhookSignature {
		/// <summary>
		/// Creates a signature for the given <paramref name="payload"/> and
		/// secret key.
		/// </summary>
		/// <param name="payload">
		/// The payload of the webhook to be signed
		/// </param>
		/// <param name="secret">
		/// The secret key used to sign the payload
		/// </param>
		/// <returns>
		/// Returns a string that represents the signature of the payload
		/// for a SendGrid webhook
		/// </returns>
		public static string Create(string payload, string secret)
			=> $"sha256={WebhookSignature.Create("sha256", payload, secret)}";
	}
}
