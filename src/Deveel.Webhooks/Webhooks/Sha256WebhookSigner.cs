using System;
using System.Security.Cryptography;
using System.Text;

namespace Deveel.Webhooks {
	public sealed class Sha256WebhookSigner : IWebhookSigner {
		string IWebhookSigner.Algorithm => WebhookSignatureAlgorithms.HmacSha256;

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
