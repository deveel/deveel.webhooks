using System;

namespace Deveel.Webhooks {
	class DefaultWebhookFilterProvider : IWebhookFilterProvider {
		public string Name { get; } = "default";

		public IWebhookFilterEvaluator GetEvaluator() => new DefaultWebhookFilterEvaluator();
	}
}
