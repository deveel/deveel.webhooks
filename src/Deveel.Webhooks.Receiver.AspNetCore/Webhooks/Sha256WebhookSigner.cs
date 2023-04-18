using System;
using System.Security.Cryptography;
using System.Text;

namespace Deveel.Webhooks {
	public class Sha256WebhookSigner : IWebhookSigner {
		public virtual string[] Algorithms => new[] { "sha256", "sha-256" };

		protected virtual byte[] ComputeHash(string jsonBody, string secret) {
			if (string.IsNullOrWhiteSpace(secret)) 
				throw new ArgumentException($"'{nameof(secret)}' cannot be null or whitespace.", nameof(secret));

			var key = Encoding.UTF8.GetBytes(secret);
			using var sha256 = new HMACSHA256(key);

			return sha256.ComputeHash(Encoding.UTF8.GetBytes(jsonBody));
		}

		protected virtual string GetSignatureString(byte[] hash) {
			return $"{Algorithms[0]}={Convert.ToBase64String(hash)}";
		}

		public virtual string SignWebhook(string jsonBody, string secret) {
			if (string.IsNullOrWhiteSpace(jsonBody)) 
				throw new ArgumentException($"'{nameof(jsonBody)}' cannot be null or whitespace.", nameof(jsonBody));
			if (string.IsNullOrWhiteSpace(secret)) 
				throw new ArgumentException($"'{nameof(secret)}' cannot be null or whitespace.", nameof(secret));

			var hash = ComputeHash(jsonBody, secret);
			return GetSignatureString(hash);
		}
	}
}
