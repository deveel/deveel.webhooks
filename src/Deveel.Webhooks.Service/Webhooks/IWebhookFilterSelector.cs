using System;

namespace Deveel.Webhooks {
	public interface IWebhookFilterSelector {
		IWebhookFilterEvaluator GetEvaluator(string filterFormat);
	}
}
