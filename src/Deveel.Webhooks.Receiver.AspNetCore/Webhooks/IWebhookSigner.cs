using System;

namespace Deveel.Webhooks {
	public interface IWebhookSigner {
		string[] Algorithms { get; }

		string SignWebhook(string jsonBody, string secret);
	}
}
