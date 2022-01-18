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
	public static class WebhookSignatureValidator {
		public static bool IsValid(string algorithm, string jsonPayload, string secret, string signature) {
			if (string.IsNullOrWhiteSpace(signature))
				throw new ArgumentException($"'{nameof(signature)}' cannot be null or whitespace.", nameof(signature));
			if (string.IsNullOrWhiteSpace(secret))
				throw new ArgumentException($"'{nameof(secret)}' cannot be null or whitespace.", nameof(secret));
			if (string.IsNullOrWhiteSpace(algorithm))
				throw new ArgumentException($"'{nameof(algorithm)}' cannot be null or whitespace.", nameof(algorithm));

			return algorithm switch {
				"sha256" => IsValidSha256(jsonPayload, signature, secret),
				_ => throw new NotSupportedException($"Te signing algorithm {algorithm} is not supported"),
			};
		}

		public static bool IsValidSha256(string content, string signature, string secret) {
			var secretBytes = Encoding.UTF8.GetBytes(secret);

			string compare;

			using (var hasher = new HMACSHA256(secretBytes)) {
				var data = Encoding.UTF8.GetBytes(content);
				var sha256 = hasher.ComputeHash(data);

				compare = BitConverter.ToString(sha256);
			}

			return string.Equals(signature, compare);
		}
	}
}
