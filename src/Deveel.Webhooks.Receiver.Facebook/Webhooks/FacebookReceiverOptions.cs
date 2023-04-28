using System;

namespace Deveel.Webhooks {
	public sealed class FacebookReceiverOptions {
		public string? AppSecret { get; set; }

		public bool? VerifySignature { get; set; }

		public string? VerifyToken { get; set; }
	}
}
