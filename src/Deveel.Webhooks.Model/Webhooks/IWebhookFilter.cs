using System;

namespace Deveel.Webhooks {
	public interface IWebhookFilter {
		string Expression { get; }

		string Format { get; }
	}
}
