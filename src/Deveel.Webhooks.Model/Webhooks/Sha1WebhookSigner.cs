using System;
using System.Security.Cryptography;

namespace Deveel.Webhooks {
    public class Sha1WebhookSigner : WebhookSignerBase {
        public override string[] Algorithms => new[] { "SHA-1", "SHA1" };

        protected override KeyedHashAlgorithm CreateHasher(byte[] key) {
            return new HMACSHA1(key);
        }
    }
}
