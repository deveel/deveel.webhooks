using System;
using System.Collections.Generic;
using System.Diagnostics;

using Deveel.Data.Memory;
using Deveel.Webhooks;
using Deveel.Webhooks.Memory;

using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

using PhoneNumbers;

namespace Deveel.Webhooks {
	public static class WebhookServiceBuilderExtensions {
		private static IWebhookServiceBuilder Use<TService, TImplementation>(this IWebhookServiceBuilder builder, ServiceLifetime lifetime = ServiceLifetime.Scoped)
			where TService : class
			where TImplementation : class, TService {
			builder.Configure(services => services.UseService<TService, TImplementation>(lifetime));
			return builder;
		}

		public static IWebhookServiceBuilder Configure(this IWebhookServiceBuilder builder, Action<WebhookConfigureOptions> configure) {
			if (configure != null)
				builder.Configure(services => services.Configure(configure));

			return builder;
		}

		public static IWebhookServiceBuilder UseNotifier<TNotifier>(this IWebhookServiceBuilder builder)
			where TNotifier : class, IWebhookNotifier
			=> builder.Use<IWebhookNotifier, TNotifier>();

		public static IWebhookServiceBuilder UseDefaultNotifier(this IWebhookServiceBuilder builder)
			=> builder.UseNotifier<WebhookNotifier>();

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
			builder.Configure(services => {
				services.TryAddScoped<IWebhookDataStrategy, DefaultWebhookDataStrategy>();
				services.AddScoped<IWebhookDataFactory, TFactory>();
			});
			return builder;
		}

		public static IWebhookServiceBuilder AddDataFactory<TFactory>(this IWebhookServiceBuilder builder, TFactory instance)
			where TFactory : class, IWebhookDataFactory {
			builder.Configure(services => {
				services.TryAddScoped<IWebhookDataStrategy, DefaultWebhookDataStrategy>();
				services.AddSingleton<IWebhookDataFactory>(instance);
			});
			return builder;
		}

		public static IWebhookServiceBuilder UseFilterEvaluator<TEvaluator>(this IWebhookServiceBuilder builder)
			where TEvaluator : class, IWebhookFilterEvaluator 
			=> builder.Use<IWebhookFilterEvaluator, TEvaluator>();

		public static IWebhookServiceBuilder UseFilterProviderRegistry<TRegistry>(this IWebhookServiceBuilder builder)
			where TRegistry : class, IWebhookFilterProviderRegistry
			=> builder.Use<IWebhookFilterProviderRegistry, TRegistry>();

		public static IWebhookServiceBuilder UseDefaultFilterProviderRegistry(this IWebhookServiceBuilder builder)
			=> builder.UseFilterProviderRegistry<DefaultWebhookFilterProviderRegistry>();

		public static IWebhookServiceBuilder AddFilterProvider<TProvider>(this IWebhookServiceBuilder builder, ServiceLifetime lifetime = ServiceLifetime.Scoped)
			where TProvider : class, IWebhookFilterProvider {
			builder.Configure(services => services.Add(new ServiceDescriptor(typeof(IWebhookFilterProvider), typeof(TProvider), lifetime)));
			builder.Configure(services => services.Add(new ServiceDescriptor(typeof(TProvider), typeof(TProvider), lifetime)));
			return builder;
		}
		
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

		public static IWebhookServiceBuilder UseInMemoryStorage(this IWebhookServiceBuilder builder, IStoreProviderState<InMemoryWebhookSubscription> state = null) {
			if (state != null)
				builder.Configure(services => services.AddSingleton(state));

			return builder.UseSubscriptionFactory<WebhookSubscriptionObjectFactory>()
				.UseSubscriptionStore<InMemoryWebhookSubscriptionStore>()
				.UseSubscriptionStoreProvider<InMemoryWebhookSubscriptionStoreProvider>();
		}

		public static IWebhookServiceBuilder UseInMemoryStorage(this IWebhookServiceBuilder builder, IDictionary<string, IEnumerable<InMemoryWebhookSubscription>> state, bool readOnly = false)
			=> builder.UseInMemoryStorage(new StoreProviderState<InMemoryWebhookSubscription>(state, readOnly));
	}
}
