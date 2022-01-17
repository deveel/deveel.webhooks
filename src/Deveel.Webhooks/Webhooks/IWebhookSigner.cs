using System;
using System.Threading.Tasks;

namespace Deveel.Webhooks {
	public interface IWebhookSigner {
		string Algorithm { get; }

		string Sign(string payloadJson, string secret);
	}
}
