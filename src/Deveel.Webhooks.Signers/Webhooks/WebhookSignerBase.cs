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
using System.Security.Cryptography;
using System.Text;

namespace Deveel.Webhooks {
    public abstract class WebhookSignerBase : IWebhookSigner {
        public abstract string[] Algorithms { get; }

        protected abstract KeyedHashAlgorithm CreateHasher(byte[] key);

        protected virtual byte[] GetKeyBytes(string secret) {
            return Encoding.UTF8.GetBytes(secret);
        }

        protected virtual byte[] GetJsonBytes(string json) => Encoding.UTF8.GetBytes(json);

        /// <summary>
        /// Computes the hash of the given <paramref name="jsonBody"/> using the
        /// provided secret,
        /// </summary>
        /// <param name="jsonBody">The JSON-formatted string that represents the webhook to sign</param>
        /// <param name="secret">A secret used as key for the signature</param>
        /// <returns>
        /// Returns a byte array representing the hash of the given body
        /// </returns>
        /// <exception cref="ArgumentException">
        /// Thrown when the <paramref name="secret"/> is <c>null</c> or empty
        /// </exception>
        protected virtual byte[] ComputeHash(string jsonBody, string secret) {
            if (String.IsNullOrWhiteSpace(secret))
                throw new ArgumentException($"'{nameof(secret)}' cannot be null or whitespace.", nameof(secret));

            var key = GetKeyBytes(secret);
            using var alg = CreateHasher(key);

            return alg.ComputeHash(GetJsonBytes(jsonBody));
        }

        /// <summary>
        /// Gets the string representation of the signature, given the hash
        /// </summary>
        /// <param name="hash">The byte hash of the signature</param>
        /// <returns>
        /// Returns a string representing the signature of the given body
        /// </returns>
        protected virtual string FormatSignature(byte[] hash) {
            return $"{Algorithms[0]}={Convert.ToHexString(hash)}";
        }

        public virtual string SignWebhook(string jsonBody, string secret) {
            if (String.IsNullOrWhiteSpace(jsonBody))
                throw new ArgumentException($"'{nameof(jsonBody)}' cannot be null or whitespace.", nameof(jsonBody));
            if (String.IsNullOrWhiteSpace(secret))
                throw new ArgumentException($"'{nameof(secret)}' cannot be null or whitespace.", nameof(secret));

            var hash = ComputeHash(jsonBody, secret);
            return FormatSignature(hash);
        }
    }
}
