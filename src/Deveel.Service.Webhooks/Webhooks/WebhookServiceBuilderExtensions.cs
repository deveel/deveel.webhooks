using System;
using System.Collections.Generic;
using System.Diagnostics;

using Deveel.Data.Memory;
using Deveel.Webhooks;
using Deveel.Webhooks.Memory;

using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

using PhoneNumbers;

namespace Deveel.Webhooks {
	public static class WebhookServiceBuilderExtensions {
		private static IWebhookServiceBuilder Use<TService, TImplementation>(this IWebhookServiceBuilder builder, ServiceLifetime lifetime = ServiceLifetime.Scoped)
			where TService : class
			where TImplementation : class, TService {
			builder.ConfigureServices(services => services.UseService<TService, TImplementation>(lifetime));
			return builder;
		}

		public static IWebhookServiceBuilder ConfigureDelivery(this IWebhookServiceBuilder builder, Action<WebhookDeliveryOptions> configure) {
			if (configure != null)
				builder.ConfigureServices(services => services.Configure(configure));

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

		public static IWebhookServiceBuilder UseDefaultNotifier(this IWebhookServiceBuilder builder)
			=> builder.UseNotifier<DefaultWebhookNotifier>();

		public static IWebhookServiceBuilder UseSubscriptionStore<TStore>(this IWebhookServiceBuilder builder)
			where TStore : class, IWebhookSubscriptionStore
			=> builder.Use<IWebhookSubscriptionStore, TStore>();

		public static IWebhookServiceBuilder UseSubscriptionStoreProvider<TProvider>(this IWebhookServiceBuilder builder)
			where TProvider : class, IWebhookSubscriptionStoreProvider
			=> builder.Use<IWebhookSubscriptionStoreProvider, TProvider>();

		public static IWebhookServiceBuilder UseSubscriptionFactory<TFactory>(this IWebhookServiceBuilder builder)
			where TFactory : class, IWebhookSubscriptionFactory
			=> builder.Use<IWebhookSubscriptionFactory, TFactory>();

		public static IWebhookServiceBuilder AddDataFactory<TFactory>(this IWebhookServiceBuilder builder)
			where TFactory : class, IWebhookDataFactory {
			builder.ConfigureServices(services => {
				services.TryAddScoped<IWebhookDataStrategy, DefaultWebhookDataStrategy>();
				services.AddScoped<IWebhookDataFactory, TFactory>();
			});
			return builder;
		}

		public static IWebhookServiceBuilder AddDataFactory<TFactory>(this IWebhookServiceBuilder builder, TFactory instance)
			where TFactory : class, IWebhookDataFactory {
			builder.ConfigureServices(services => {
				services.TryAddScoped<IWebhookDataStrategy, DefaultWebhookDataStrategy>();
				services.AddSingleton<IWebhookDataFactory>(instance);
			});
			return builder;
		}

		public static IWebhookServiceBuilder UseFilterEvaluator<TEvaluator>(this IWebhookServiceBuilder builder)
			where TEvaluator : class, IWebhookFilterEvaluator 
			=> builder.Use<IWebhookFilterEvaluator, TEvaluator>();

		public static IWebhookServiceBuilder UseFilterFactory<TFactory>(this IWebhookServiceBuilder builder)
			where TFactory : class, IWebhookFilterRequestFactory
			=> builder.Use<IWebhookFilterRequestFactory, TFactory>();
		
		public static IWebhookServiceBuilder UseSubscriptionResolver<TResolver>(this IWebhookServiceBuilder builder)
			where TResolver : class, IWebhookSubscriptionResolver 
			=> builder.Use<IWebhookSubscriptionResolver, TResolver>();

		public static IWebhookServiceBuilder UseDefaultSubscriptionResolver(this IWebhookServiceBuilder builder)
			=> builder.UseSubscriptionResolver<DefaultWebhookSubscriptionResolver>();

		public static IWebhookServiceBuilder UseDefaultFilterEvaluator(this IWebhookServiceBuilder builder)
			=> builder.UseFilterEvaluator<DefaultWebhookFilterEvaluator>();

		public static IWebhookServiceBuilder UseSender<TSender>(this IWebhookServiceBuilder builder)
			where TSender : class, IWebhookSender
			=> builder.Use<IWebhookSender, TSender>();

		public static IWebhookServiceBuilder UseDefaultSender(this IWebhookServiceBuilder builder)
			=> builder.UseSender<DefaultWebhookSender>();

		public static IWebhookServiceBuilder UseSubscriptionManager<TManager>(this IWebhookServiceBuilder builder)
			where TManager : class, IWebhookSubscriptionManager
			=> builder.Use<IWebhookSubscriptionManager, TManager>();

		public static IWebhookServiceBuilder UseDefaultSubscriptionManager(this IWebhookServiceBuilder builder)
			=> builder.UseSubscriptionManager<DefaultWebhookSubscriptionManager>();

		public static IWebhookServiceBuilder UseInMemoryStorage(this IWebhookServiceBuilder builder, IStoreState<InMemoryWebhookSubscription> state = null) {
			if (state != null)
				builder.ConfigureServices(services => services.AddSingleton(state));

			return builder.UseSubscriptionFactory<WebhookSubscriptionObjectFactory>()
				.UseSubscriptionStore<InMemoryWebhookSubscriptionStore>()
				.UseSubscriptionStoreProvider<InMemoryWebhookSubscriptionStoreProvider>();
		}

		public static IWebhookServiceBuilder UseInMemoryStorage(this IWebhookServiceBuilder builder, IEnumerable<InMemoryWebhookSubscription> state, bool readOnly = false)
			=> builder.UseInMemoryStorage(readOnly ? StoreState<InMemoryWebhookSubscription>.ReadOnly(state) : new StoreState<InMemoryWebhookSubscription>(state));
	}
}
