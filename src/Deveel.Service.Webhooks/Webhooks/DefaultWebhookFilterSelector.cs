using System;
using System.Linq;

using Microsoft.Extensions.DependencyInjection;

namespace Deveel.Webhooks {
	class DefaultWebhookFilterSelector : IWebhookFilterSelector {
		private readonly IServiceProvider provider;

		public DefaultWebhookFilterSelector(IServiceProvider provider) {
			this.provider = provider;
		}

		public IWebhookFilterEvaluator GetEvaluator(string filterFormat) {
			if (String.IsNullOrEmpty(filterFormat))
				return null;

			var evals = provider.GetServices<IWebhookFilterEvaluator>();
			return evals?.FirstOrDefault(x => x.Format == filterFormat);
		}
	}
}
