using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Deveel.Webhooks {
	readonly struct FacebookVerificationResult : IWebhookVerificationResult {
		public FacebookVerificationResult(bool valid, bool isVerified, string? challenge = null) {
			IsValid = valid;
			IsVerified = isVerified;
			Challenge = challenge;
		}

		public bool IsVerified { get; }

		public string? Challenge { get; }

		public bool IsValid { get; }
	}
}
