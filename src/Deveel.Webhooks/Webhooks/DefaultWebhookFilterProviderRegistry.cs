using System;
using System.Linq;

using Microsoft.Extensions.DependencyInjection;

namespace Deveel.Webhooks {
	public sealed class DefaultWebhookFilterProviderRegistry : IWebhookFilterProviderRegistry {
		private IServiceProvider provider;

		public DefaultWebhookFilterProviderRegistry(IServiceProvider provider) {
			this.provider = provider;
		}

		public IWebhookFilterProvider GetProvider(string name) {
			var services = provider.GetServices<IWebhookFilterProvider>();
			return services.FirstOrDefault(x => x.Name == name);
		}
	}
}
