using System;

namespace Deveel.Webhooks {
	public interface IWebhookFilter {
		string Expression { get; }

		string Provider { get; }

		string ExpressionFormat { get; }
	}
}
