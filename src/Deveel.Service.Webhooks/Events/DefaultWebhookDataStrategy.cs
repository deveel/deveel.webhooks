using System;
using System.Collections.Generic;
using System.Linq;

namespace Deveel.Webhooks {
	public class DefaultWebhookDataStrategy : IWebhookDataStrategy {
		private readonly IEnumerable<IWebhookDataFactory> providers;

		public DefaultWebhookDataStrategy(IEnumerable<IWebhookDataFactory> providers) {
			this.providers = providers;
		}

		public IWebhookDataFactory GetDataFactory(string eventType)
			=> providers?.FirstOrDefault(x => x.AppliesTo(eventType));
	}
}
