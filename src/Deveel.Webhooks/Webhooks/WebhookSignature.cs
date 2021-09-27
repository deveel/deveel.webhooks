using System;

namespace Deveel.Webhooks {
	public static class WebhookSignature {
		public static IWebhookSignatureProvider Sha256 => new Sha256WebhookSignatureProvider();

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
