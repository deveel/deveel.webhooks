﻿using System;
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
