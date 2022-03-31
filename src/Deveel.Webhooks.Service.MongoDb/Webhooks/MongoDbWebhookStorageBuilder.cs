using System;

using Deveel.Data;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace Deveel.Webhooks {
	public sealed class MongoDbWebhookStorageBuilder<TSubscription> where TSubscription : MongoDbWebhookSubscription {
		private readonly WebhookServiceBuilder<TSubscription> builder;

		internal MongoDbWebhookStorageBuilder(WebhookServiceBuilder<TSubscription> builder) {
			this.builder = builder;

			AddDefaultStorage();
		}

		private IServiceCollection Services => builder.Services;

		private static IServiceCollection ConfigureMongoDbOptions(IServiceCollection services, string sectionName, string connectionStringName = null) {
			services.AddOptions<MongoDbOptions>()
				.Configure<IConfiguration>((options, config) => {
					var section = config.GetSection(sectionName);
					if (section != null)
						section.Bind(options);

					if (!string.IsNullOrWhiteSpace(connectionStringName)) options.ConnectionString = config.GetConnectionString(connectionStringName);
				});

			return services;
		}

		private IServiceCollection AddSubscriptionFactory(IServiceCollection services) {
			services.TryAddSingleton<IWebhookSubscriptionFactory<TSubscription>, MongoDbWebhookSubscriptionFactory<TSubscription>>();
			services.TryAddSingleton<IWebhookSubscriptionFactory<MongoDbWebhookSubscription>, MongoDbWebhookSubscriptionFactory>();
			services.AddSingleton<MongoDbWebhookSubscriptionFactory>();

			return services;
		}

		private void AddDefaultStorage() {
				AddSubscriptionFactory(Services);

				//Services.TryAddScoped<IWebhookSubscriptionStore, MongoDbWebhookSubscriptionStrore>();
				//Services.TryAddScoped<IWebhookSubscriptionStore<IWebhookSubscription>, MongoDbWebhookSubscriptionStrore>();
				Services.TryAddScoped<IWebhookSubscriptionStore<MongoDbWebhookSubscription>, MongoDbWebhookSubscriptionStrore>();

				Services.TryAddScoped<IWebhookSubscriptionStoreProvider, MongoDbWebhookSubscriptionStoreProvider>();
				Services.TryAddScoped<IWebhookSubscriptionStoreProvider<IWebhookSubscription>, MongoDbWebhookSubscriptionStoreProvider>();
				Services.TryAddScoped<IWebhookSubscriptionStoreProvider<MongoDbWebhookSubscription>, MongoDbWebhookSubscriptionStoreProvider>();
		}

		public MongoDbWebhookStorageBuilder<TSubscription> Configure(Action<IMongoDbOptionBuilder> configure) {
			Services.AddOptions<MongoDbOptions>()
				.Configure(options => {
					var optionsBuilder = new MongoDbOptionBuilder(options);
					configure(optionsBuilder);
				});

			return this;
		}

		public MongoDbWebhookStorageBuilder<TSubscription> Configure(Action<MongoDbOptions> configure) {
			Services.AddOptions<MongoDbOptions>().Configure(configure);

			return this;
		}

		public MongoDbWebhookStorageBuilder<TSubscription> Configure(string sectionName, string connectionStringName = null) {
			ConfigureMongoDbOptions(Services, sectionName, connectionStringName);

			return this;
		}

		public MongoDbWebhookStorageBuilder<TSubscription> UseSubscriptionStore<TStore>()
			where TStore : MongoDbWebhookSubscriptionStrore {
			AddSubscriptionFactory(Services);
			Services.AddSingleton<IWebhookSubscriptionStore, TStore>();
			Services.AddSingleton<IWebhookSubscriptionStore<IWebhookSubscription>, TStore>();
			Services.AddSingleton<IWebhookSubscriptionStore<MongoDbWebhookSubscription>, TStore>();

			return this;
		}

		public MongoDbWebhookStorageBuilder<TSubscription> UseSubscriptionStoreProvider<TProvider>()
			where TProvider : MongoDbWebhookSubscriptionStoreProvider {
			AddSubscriptionFactory(Services);
			Services.AddSingleton<IWebhookSubscriptionStoreProvider, TProvider>();
			Services.AddSingleton<IWebhookSubscriptionStoreProvider<IWebhookSubscription>, TProvider>();
			Services.AddSingleton<IWebhookSubscriptionStoreProvider<MongoDbWebhookSubscription>, TProvider>();

			return this;
		}

	}
}
