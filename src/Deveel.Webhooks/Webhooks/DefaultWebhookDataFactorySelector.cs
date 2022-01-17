using System;
using System.Collections.Generic;
using System.Linq;

namespace Deveel.Webhooks {
	class DefaultWebhookDataFactorySelector : IWebhookDataFactorySelector {
		private readonly IEnumerable<IWebhookDataFactory> providers;

		public DefaultWebhookDataFactorySelector(IEnumerable<IWebhookDataFactory> providers) {
			this.providers = providers;
		}

		public IWebhookDataFactory GetDataFactory(EventInfo eventInfo)
			=> providers?.FirstOrDefault(x => x.Handles(eventInfo));
	}
}
