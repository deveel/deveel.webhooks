using System;

using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace Deveel.Webhooks {
	public static class WebhookServiceBuilderExtensions {
		public static IWebhookServiceBuilder Use<TService, TImplementation>(this IWebhookServiceBuilder builder, ServiceLifetime lifetime = ServiceLifetime.Scoped)
			where TService : class
			where TImplementation : class, TService {
			builder.ConfigureServices(services => services.UseService<TService, TImplementation>(lifetime));
			return builder;
		}

		public static IWebhookServiceBuilder ConfigureDelivery(this IWebhookServiceBuilder builder, Action<WebhookDeliveryOptions> configure) {
			if (configure != null)
				builder.ConfigureServices(services => services.AddOptions<WebhookDeliveryOptions>().Configure(configure));

			return builder;
		}

		public static IWebhookServiceBuilder ConfigureDelivery(this IWebhookServiceBuilder builder, WebhookDeliveryOptions options) {
			builder.ConfigureServices(services => services.AddSingleton(options));
			return builder;
		}

		public static IWebhookServiceBuilder ConfigureDelivery(this IWebhookServiceBuilder builder, IConfiguration configuration) {
			builder.ConfigureServices(services => services.AddOptions<WebhookDeliveryOptions>().Bind(configuration));
			return builder;
		}

		public static IWebhookServiceBuilder UseNotifier<TNotifier>(this IWebhookServiceBuilder builder)
			where TNotifier : class, IWebhookNotifier
			=> builder.Use<IWebhookNotifier, TNotifier>();

		public static IWebhookServiceBuilder UseNotifier(this IWebhookServiceBuilder builder)
			=> builder.UseNotifier<WebhookNotifier>();

		public static IWebhookServiceBuilder AddDataFactory<TFactory>(this IWebhookServiceBuilder builder)
			where TFactory : class, IWebhookDataFactory {
			builder.ConfigureServices(services => {
				services.TryAddScoped<IWebhookDataFactorySelector, DefaultWebhookDataFactorySelector>();
				services.AddScoped<IWebhookDataFactory, TFactory>();
			});
			return builder;
		}

		public static IWebhookServiceBuilder AddFilterEvaluator<TEvaluator>(this IWebhookServiceBuilder builder, ServiceLifetime lifetime = ServiceLifetime.Transient)
			where TEvaluator : class, IWebhookFilterEvaluator {
			builder.ConfigureServices(services => {
				services.Add(new ServiceDescriptor(typeof(IWebhookFilterEvaluator), typeof(TEvaluator), lifetime));
				services.Add(new ServiceDescriptor(typeof(TEvaluator), typeof(TEvaluator), lifetime));
			});

			return builder;
		}

		public static IWebhookServiceBuilder UseSubscriptionResolver<TResolver>(this IWebhookServiceBuilder builder)
			where TResolver : class, IWebhookSubscriptionResolver
			=> builder.Use<IWebhookSubscriptionResolver, TResolver>();

		public static IWebhookServiceBuilder UseSender<TSender>(this IWebhookServiceBuilder builder)
			where TSender : class, IWebhookSender
			=> builder.Use<IWebhookSender, TSender>();

		public static IWebhookServiceBuilder UseDefaultSender(this IWebhookServiceBuilder builder)
			=> builder.UseSender<WebhookSender>();
	}
}
