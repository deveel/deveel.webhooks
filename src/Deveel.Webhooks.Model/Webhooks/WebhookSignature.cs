using System;

namespace Deveel.Webhooks {
    public static class WebhookSignature {
        public static string Create(string algorithm, string json, string secret) {
            IWebhookSigner signer;

            switch (algorithm.ToUpperInvariant()) {
                case "SHA1":
                case "SHA-1":
                    signer = new Sha1WebhookSigner();
                    break;
                case "SHA256":
                case "SHA-256":
                    signer = new Sha256WebhookSigner();
                    break;
                default:
                    throw new NotSupportedException($"The algorithm '{algorithm}' is not supported.");
            }

            return signer.SignWebhook(json, secret);
        }
    }
}
