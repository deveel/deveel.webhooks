using System;
using System.Threading.Tasks;

namespace Deveel.Webhooks {
	public interface IWebhookSignatureProvider {
		string Algorithm { get; }

		string Sign(string payloadJson, string secret);
	}
}
