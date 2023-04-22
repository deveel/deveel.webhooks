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

using System.Security.Cryptography;

namespace Deveel.Webhooks {
    /// <summary>
    /// A default implementation of the <see cref="IWebhookSigner"/> that handles
    /// a typical webhook signature using the SHA-256 algorithm.
    /// </summary>
    public class Sha256WebhookSigner : WebhookSignerBase {
		/// <inheritdoc/>
		public override string[] Algorithms => new[] { "sha256", "sha-256" };

		/// <summary>
		/// Creates a new instance of the <see cref="HMACSHA256"/> algorithm
		/// </summary>
		/// <param name="key">
		/// The key to use to create the hasher.
		/// </param>
		/// <returns>
		/// Returns a new instance of <see cref="HMACSHA256"/>.
		/// </returns>
        protected override KeyedHashAlgorithm CreateHasher(byte[] key) => new HMACSHA256(key);
    }
}
