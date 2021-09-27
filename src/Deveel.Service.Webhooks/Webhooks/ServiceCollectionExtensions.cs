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

			services.TryAddScoped<IWebhookNotifier, DefaultWebhookNotifier>();
			services.AddScoped<DefaultWebhookNotifier>();

			services.TryAddScoped<IWebhookSubscriptionManager, DefaultWebhookSubscriptionManager>();
			services.AddScoped<DefaultWebhookSubscriptionManager>();

			services.TryAddScoped<IWebhookSender, DefaultWebhookSender>();
			services.AddScoped<DefaultWebhookSender>();

			services.TryAddScoped<IWebhookSubscriptionResolver, DefaultWebhookSubscriptionResolver>();
			services.AddScoped<DefaultWebhookSubscriptionResolver>();

			services.TryAddScoped<IWebhookFilterEvaluator, DefaultWebhookFilterEvaluator>();
			services.AddScoped<DefaultWebhookFilterEvaluator>();

			services.TryAddScoped<IWebhookFilterRequestFactory, DefaultWebookFilterRequestFactory>();
			services.AddScoped<DefaultWebookFilterRequestFactory>();

			services.AddSingleton<IWebhookSignatureProvider, Sha256WebhookSignatureProvider>();
			services.AddSingleton<Sha256WebhookSignatureProvider>();

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
