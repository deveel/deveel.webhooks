using System;
using System.Collections.Generic;

using Deveel.Data.Memory;
using Deveel.Webhooks;
using Deveel.Webhooks.Memory;

using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace Deveel.Webhooks {
	public static class WebhookConfigurationBuilderExtensions {
		public static IWebhookConfigurationBuilder Configure(this IWebhookConfigurationBuilder builder, Action<WebhookConfigureOptions> configure) {
			if (configure != null)
				builder.Services.Configure(configure);

			return builder;
		}

		public static IWebhookConfigurationBuilder UseSubscriptionStore<TStore>(this IWebhookConfigurationBuilder builder)
			where TStore : class, IWebhookSubscriptionStore {
			builder.Services.RemoveAll<IWebhookSubscriptionStore>();
			builder.Services.AddScoped<IWebhookSubscriptionStore, TStore>();
			return builder;
		}

		public static IWebhookConfigurationBuilder UseSubscriptionStoreProvider<TProvider>(this IWebhookConfigurationBuilder builder)
			where TProvider : class, IWebhookSubscriptionStoreProvider {
			builder.Services.RemoveAll<IWebhookSubscriptionStoreProvider>();
			builder.Services.AddScoped<IWebhookSubscriptionStoreProvider, TProvider>();
			return builder;
		}

		public static IWebhookConfigurationBuilder UseSubscriptionFactory<TFactory>(this IWebhookConfigurationBuilder builder)
			where TFactory : class, IWebhookSubscriptionFactory {
			builder.Services.RemoveAll<IWebhookSubscriptionFactory>();
			builder.Services.AddScoped<IWebhookSubscriptionFactory, TFactory>();
			return builder;
		}

		public static IWebhookConfigurationBuilder AddDataFactory<TFactory>(this IWebhookConfigurationBuilder builder)
			where TFactory : class, IWebhookDataFactory {
			builder.Services.TryAddScoped<IWebhookDataStrategy, DefaultWebhookDataStrategy>();
			builder.Services.AddScoped<IWebhookDataFactory, TFactory>();
			return builder;
		}

		public static IWebhookConfigurationBuilder AddDataFactory<TFactory>(this IWebhookConfigurationBuilder builder, TFactory instance)
			where TFactory : class, IWebhookDataFactory {
			builder.Services.TryAddScoped<IWebhookDataStrategy, DefaultWebhookDataStrategy>();
			builder.Services.AddSingleton<IWebhookDataFactory>(instance);
			return builder;
		}

		public static IWebhookConfigurationBuilder UseInMemoryStorage(this IWebhookConfigurationBuilder builder, IStoreProviderState<InMemoryWebhookSubscription> state = null) {
			if (state != null)
				builder.Services.AddSingleton(state);

			return builder.UseSubscriptionFactory<WebhookSubscriptionObjectFactory>()
				.UseSubscriptionStore<InMemoryWebhookSubscriptionStore>()
				.UseSubscriptionStoreProvider<InMemoryWebhookSubscriptionStoreProvider>();
		}

		public static IWebhookConfigurationBuilder UseInMemoryStorage(this IWebhookConfigurationBuilder builder, IDictionary<string, IEnumerable<InMemoryWebhookSubscription>> state, bool readOnly = false)
			=> builder.UseInMemoryStorage(new StoreProviderState<InMemoryWebhookSubscription>(state, readOnly));
	}
}
