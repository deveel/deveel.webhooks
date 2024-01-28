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
using System.Security.Cryptography;
using System.Text;

namespace Deveel.Webhooks {
	/// <summary>
	/// An abstract implementation of <see cref="IWebhookSigner"/> that provides
	/// a generic method to sign the payloads.
	/// </summary>
    public abstract class WebhookSignerBase : IWebhookSigner {
		/// <summary>
		/// Initializes a new instance of the <see cref="WebhookSignerBase"/> class
		/// with the given encoding name used for the secret key.
		/// </summary>
		/// <param name="keyEncodingName">
		/// The name of the encoding used to create the secret key (eg. UTF-8, ASCII).
		/// </param>
		/// <exception cref="ArgumentException">
		/// Thrown when the given encoding name is null or whitespace.
		/// </exception>
		protected WebhookSignerBase(string keyEncodingName) {
			if (string.IsNullOrWhiteSpace(keyEncodingName)) 
				throw new ArgumentException($"'{nameof(keyEncodingName)}' cannot be null or whitespace.", nameof(keyEncodingName));

			KeyEncodingName = keyEncodingName;
		}

		/// <summary>
		/// Gets the name of the encoding used to create the secret key
		/// (by default is UTF-8).
		/// </summary>
		protected string KeyEncodingName { get; } = "UTF-8";

		/// <summary>
		/// When overridden, gets the names of the algorithms supported by this
		/// signer instance.
		/// </summary>
		public abstract string[] Algorithms { get; }

		/// <summary>
		/// When overridden, creates a new instance of the <see cref="KeyedHashAlgorithm"/>
		/// used to compute the signature.
		/// </summary>
		/// <param name="key">
		/// The key to use to create the hasher.
		/// </param>
		/// <returns>
		/// Returns a new instance of <see cref="KeyedHashAlgorithm"/>.
		/// </returns>
        protected abstract KeyedHashAlgorithm CreateHasher(byte[] key);

		/// <summary>
		/// Creates the byte array representation of the secret key.
		/// </summary>
		/// <param name="secret">
		/// The secret key to use to create the hasher.
		/// </param>
		/// <returns>
		/// Returns a byte array representing the secret key.
		/// </returns>
        protected virtual byte[] GetKeyBytes(string secret) {
            return Encoding.GetEncoding(KeyEncodingName).GetBytes(secret);
        }

		/// <summary>
		/// Gets the byte array representation of the given <paramref name="webhookBody"/>.
		/// </summary>
		/// <param name="webhookBody">
		/// The string that represents the webhook to sign.
		/// </param>
		/// <returns>
		/// Returns a byte array representing the payload of the webhook.
		/// </returns>
        protected virtual byte[] GetPayloadBytes(string webhookBody) 
			=> Encoding.UTF8.GetBytes(webhookBody);

        /// <summary>
        /// Computes the hash of the given <paramref name="webhookBody"/> using the
        /// provided secret,
        /// </summary>
        /// <param name="webhookBody">The string that represents the webhook to sign</param>
        /// <param name="secret">A secret used as key for the signature</param>
        /// <returns>
        /// Returns a byte array representing the hash of the given body
        /// </returns>
        /// <exception cref="ArgumentException">
        /// Thrown when the <paramref name="secret"/> is <c>null</c> or empty
        /// </exception>
        protected virtual byte[] ComputeHash(string webhookBody, string secret) {
            if (String.IsNullOrWhiteSpace(secret))
                throw new ArgumentException($"'{nameof(secret)}' cannot be null or whitespace.", nameof(secret));

            var key = GetKeyBytes(secret);
            using var alg = CreateHasher(key);

            return alg.ComputeHash(GetPayloadBytes(webhookBody));
        }

        /// <summary>
        /// Gets the string representation of the signature, given the hash
        /// </summary>
        /// <param name="hash">The byte hash of the signature</param>
        /// <returns>
        /// Returns a string representing the signature of the given body
        /// </returns>
        protected virtual string FormatSignature(byte[] hash) {
			// TODO: should we prepend the algorithm name here?
            return Convert.ToHexString(hash);
        }

		/// <inheritdoc />
        public virtual string SignWebhook(string webhookBody, string secret) {
            if (String.IsNullOrWhiteSpace(webhookBody))
                throw new ArgumentException($"'{nameof(webhookBody)}' cannot be null or whitespace.", nameof(webhookBody));
            if (String.IsNullOrWhiteSpace(secret))
                throw new ArgumentException($"'{nameof(secret)}' cannot be null or whitespace.", nameof(secret));

            var hash = ComputeHash(webhookBody, secret);
            return FormatSignature(hash);
        }
    }
}
