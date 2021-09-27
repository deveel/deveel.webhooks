using System;

using Microsoft.Extensions.DependencyInjection;

namespace Deveel.Webhooks {
	public static class ServiceCollectionExtensions {
		public static IServiceCollection AddWebhookReceivers(this IServiceCollection services, Action<IWebhookReceiverConfigurationBuilder> configure = null) {


			if (configure != null) {
				var builder = new WebhookReceiverConfigurationBuilder(services);
				configure(builder);
			}

			return services;
		}

		class WebhookReceiverConfigurationBuilder : IWebhookReceiverConfigurationBuilder {
			public WebhookReceiverConfigurationBuilder(IServiceCollection services) {
				Services = services;
			}

			public IServiceCollection Services { get; }

			public IWebhookReceiverConfigurationBuilder ConfigureServices(Action<IServiceCollection> configure) {
				if (configure != null)
					configure(Services);

				return this;
			}
		}
	}
}
