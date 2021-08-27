using System;
using System.Runtime.CompilerServices;

using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace Deveel.Webhooks {
	public static class ServiceCollectionExtensions {
		internal static IServiceCollection UseService<TService, TImplementation>(this IServiceCollection services, ServiceLifetime lifetime = ServiceLifetime.Scoped)
			where TService : class
			where TImplementation : class, TService {
			services.RemoveAll<TService>();
			services.Add(new ServiceDescriptor(typeof(TService), typeof(TImplementation), lifetime));
			services.Add(new ServiceDescriptor(typeof(TImplementation), typeof(TImplementation), lifetime));

			return services;
		}

		public static IServiceCollection AddWebhooks(this IServiceCollection services, Action<IWebhookServiceBuilder> configure = null) {
			services.TryAddScoped<IWebhookDataStrategy, DefaultWebhookDataStrategy>();

			var builder = new WebhookConfigurationBuilderImpl(services);

			builder.UseDefaultSubscriptionManager();
			builder.UseDefaultSender();

			builder.UseDefaultFilterEvaluator();
			builder.UseDefaultSubscriptionResolver();
			builder.UseDefaultNotifier();

			if (configure != null) {
				configure(builder);
			}

			return services;
		}

		class WebhookConfigurationBuilderImpl : IWebhookServiceBuilder {
			public IServiceCollection Services { get; }

			public void Configure(Action<IServiceCollection> configure) {
				if (configure != null)
					configure(Services);
			}

			public WebhookConfigurationBuilderImpl(IServiceCollection services) {
				Services = services;
			}
		}
	}
}
