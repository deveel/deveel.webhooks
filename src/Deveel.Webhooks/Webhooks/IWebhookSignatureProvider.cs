using System;
using System.Collections.Generic;
using System.Text;

namespace Deveel.Webhooks {
	public interface IWebhookSignatureProvider {
		string Algorithm { get; }

		string Sign(string payloadJson, string secret);
	}
}
