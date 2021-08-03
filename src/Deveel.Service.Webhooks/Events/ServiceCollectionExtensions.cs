using System;

using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace Deveel.Webhooks {
	public static class ServiceCollectionExtensions {
		public static IServiceCollection AddWebhooks(this IServiceCollection services, Action<IWebhookConfigurationBuilder> configure = null) {
			services.TryAddScoped<IWebhookManager, DefaultWebhookManager>();
			services.TryAddScoped<IWebhookDataStrategy, DefaultWebhookDataStrategy>();
			services.TryAddScoped<IWebhookSender, DefaultWebhookSender>();

			if (configure != null) {
				var builder = new WebhookConfigurationBuilderImpl(services);
				configure(builder);
			}

			return services;
		}

		class WebhookConfigurationBuilderImpl : IWebhookConfigurationBuilder {
			public IServiceCollection Services { get; }

			public WebhookConfigurationBuilderImpl(IServiceCollection services) {
				Services = services;
			}
		}
	}
}
