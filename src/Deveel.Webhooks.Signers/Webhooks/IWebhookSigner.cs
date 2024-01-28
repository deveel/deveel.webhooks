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

using System;

namespace Deveel.Webhooks {
	/// <summary>
	/// Provides functions for the signing of a webhook payload
	/// </summary>
	public interface IWebhookSigner {
		/// <summary>
		/// Gets the list of algorithms supported by this signer
		/// </summary>
		string[] Algorithms { get; }

		/// <summary>
		/// Signs the given JSON body using the provided secret
		/// as a key for the signature
		/// </summary>
		/// <param name="webhookBody">The string represenation of the body of a webhook to sign</param>
		/// <param name="secret">The secret used as a key for the signature</param>
		/// <remarks>
		/// A typical implementation of this method would return a string that
		/// contains the signature, prefixed by the algorithm used to sign the
		/// webhook in the format <c>[algorithm]=[signature]</c>.
		/// </remarks>
		/// <returns>
		/// Returns a string representing the signature of the given body
		/// </returns>
		string SignWebhook(string webhookBody, string secret);
	}
}
