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

namespace Deveel.Webhooks {
	public static class WebhookSignature {
		public static IWebhookSigner Sha256 => new Sha256WebhookSigner();

		public static string Sign(string algorithm, string jsonPayload, string secret) {
			if (string.IsNullOrWhiteSpace(algorithm))
				throw new ArgumentException($"'{nameof(algorithm)}' cannot be null or whitespace.", nameof(algorithm));
			if (string.IsNullOrWhiteSpace(jsonPayload))
				throw new ArgumentException($"'{nameof(jsonPayload)}' cannot be null or whitespace.", nameof(jsonPayload));
			if (string.IsNullOrWhiteSpace(secret))
				throw new ArgumentException($"'{nameof(secret)}' cannot be null or whitespace.", nameof(secret));

			switch (algorithm) {
				case WebhookSignatureAlgorithms.HmacSha256:
					return Sha256.Sign(jsonPayload, secret);
				default:
					throw new NotSupportedException($"Signing algorithm {algorithm} is not supported");
			}
		}
	}
}
