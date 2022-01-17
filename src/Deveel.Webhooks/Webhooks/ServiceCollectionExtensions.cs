using System;
using System.Collections.Generic;
using System.Text;

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
			services.TryAddScoped<IWebhookDataFactorySelector, DefaultWebhookDataFactorySelector>();

			services.TryAddScoped<IWebhookNotifier, WebhookNotifier>();
			services.AddScoped<WebhookNotifier>();

			services.TryAddScoped<IWebhookSender, WebhookSender>();
			services.AddScoped<WebhookSender>();

			services.TryAddScoped<IWebhookFilterSelector, DefaultWebhookFilterSelector>();

			services.AddSingleton<IWebhookSigner, Sha256WebhookSigner>();
			services.AddSingleton<Sha256WebhookSigner>();

			var builder = new WebhookConfigurationBuilderImpl(services);

			configure?.Invoke(builder);

			return services;
		}

		class WebhookConfigurationBuilderImpl : IWebhookServiceBuilder {
			public IServiceCollection Services { get; }

			public void ConfigureServices(Action<IServiceCollection> configure) {
				configure?.Invoke(Services);
			}

			public WebhookConfigurationBuilderImpl(IServiceCollection services) {
				Services = services;
			}
		}

	}
}