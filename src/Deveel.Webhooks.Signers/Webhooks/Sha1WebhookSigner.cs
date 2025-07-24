// Copyright 2022-2025 Antonello Provenzano
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

using System.Security.Cryptography;

namespace Deveel.Webhooks {
	/// <summary>
	/// An implementation of <see cref="IWebhookSigner"/> that uses the
	/// SHA-1 algorithm to sign the payloads.
	/// </summary>
	public class Sha1WebhookSigner : WebhookSignerBase {
		/// <summary>
		/// Constructs a new instance of the signer with the given encoding
		/// name for the secret key.
		/// </summary>
		/// <param name="keyEncodingName"></param>
		public Sha1WebhookSigner(string keyEncodingName) 
			: base(keyEncodingName) {
		}

		/// <summary>
		/// Constructs a new instance of the signer with the default encoding
		/// name set to <c>ASCII</c>.
		/// </summary>
		public Sha1WebhookSigner()
			: this("ASCII") {
		}

		/// <summary>
		/// Gets the name of the algorithm used by this signer.
		/// </summary>
		public override string[] Algorithms => new[] { "SHA-1", "SHA1" };

		/// <summary>
		/// Creates a new instance of the <see cref="HMACSHA1"/> algorithm
		/// </summary>
		/// <param name="key">
		/// The key to use to create the hasher.
		/// </param>
		/// <returns>
		/// Returns a new instance of <see cref="HMACSHA1"/>.
		/// </returns>
        protected override KeyedHashAlgorithm CreateHasher(byte[] key) {
            return new HMACSHA1(key);
        }
    }
}
