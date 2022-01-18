// Copyright 2022 Deveel
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
using System.Security.Cryptography;
using System.Text;

namespace Deveel.Webhooks {
	/// <summary>
	/// An implementation that compute webhook signatures
	/// using the HMAC-SHA-256 algorithm.
	/// </summary>
	public sealed class Sha256WebhookSigner : IWebhookSigner {
		string IWebhookSigner.Algorithm => WebhookSignatureAlgorithms.HmacSha256;

		/// <inheritdoc/>
		public string Sign(string serializedBody, string secret) {
			var secretBytes = Encoding.UTF8.GetBytes(secret);

			string signature;

			using (var hasher = new HMACSHA256(secretBytes)) {
				var data = Encoding.UTF8.GetBytes(serializedBody);
				var sha256 = hasher.ComputeHash(data);

				signature = BitConverter.ToString(sha256);
			}

			return signature;
		}
	}
}
