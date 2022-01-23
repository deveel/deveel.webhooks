using System;

using Deveel.Data;

using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace Deveel.Webhooks.Storage {
	class MongoDbWebhookStorageBuilderImpl : IMongoDbWebhookStorageBuilder {
		private readonly IWebhookServiceBuilder serviceBuilder;

		public MongoDbWebhookStorageBuilderImpl(IWebhookServiceBuilder serviceBuilder) {
			this.serviceBuilder = serviceBuilder;

			AddDefaultStorage();
		}

		private static IServiceCollection ConfigureMongoDbOptions(IServiceCollection services, string sectionName, string connectionStringName = null) {
			services.AddOptions<MongoDbOptions>()
				.Configure<IConfiguration>((options, config) => {
					var section = config.GetSection(sectionName);
					if (section != null)
						section.Bind(options);

					if (!String.IsNullOrWhiteSpace(connectionStringName)) {
						options.ConnectionString = config.GetConnectionString(connectionStringName);
					}
				});

			return services;
		}

		private static IServiceCollection AddSubscriptionFactory(IServiceCollection services) {
			services.TryAddSingleton<IWebhookSubscriptionFactory, MongoDbWebhookSubscriptionFactory>();
			services.TryAddSingleton<IWebhookSubscriptionFactory<MongoDbWebhookSubscription>, MongoDbWebhookSubscriptionFactory>();
			services.TryAddSingleton<IWebhookSubscriptionFactory<IWebhookSubscription>, MongoDbWebhookSubscriptionFactory>();
			services.AddSingleton<MongoDbWebhookSubscriptionFactory>();

			return services;
		}

		private void AddDefaultStorage() {
			serviceBuilder.ConfigureServices(services => {
				AddSubscriptionFactory(services);

				services.TryAddScoped<IWebhookSubscriptionStore, MongoDbWebhookSubscriptionStrore>();
				services.TryAddScoped<IWebhookSubscriptionStore<IWebhookSubscription>, MongoDbWebhookSubscriptionStrore>();
				services.TryAddScoped<IWebhookSubscriptionStore<MongoDbWebhookSubscription>, MongoDbWebhookSubscriptionStrore>();

				services.TryAddScoped<IWebhookSubscriptionStoreProvider, MongoDbWebhookSubscriptionStoreProvider>();
				services.TryAddScoped<IWebhookSubscriptionStoreProvider<IWebhookSubscription>, MongoDbWebhookSubscriptionStoreProvider>();
				services.TryAddScoped<IWebhookSubscriptionStoreProvider<MongoDbWebhookSubscription>, MongoDbWebhookSubscriptionStoreProvider>();
			});
		}

		public IMongoDbWebhookStorageBuilder Configure(Action<IMongoDbOptionBuilder> configure) {
			serviceBuilder.ConfigureServices(services => {
				services.AddOptions<MongoDbOptions>()
				.Configure(options => {
					var optionsBuilder = new MongoDbOptionBuilder(options);
					configure(optionsBuilder);
				});
			});

			return this;
		}

		public IMongoDbWebhookStorageBuilder Configure(Action<MongoDbOptions> configure) {
			serviceBuilder.ConfigureServices(services => {
				services.AddOptions<MongoDbOptions>().Configure(configure);
			});

			return this;
		}

		public IMongoDbWebhookStorageBuilder Configure(string sectionName, string connectionStringName = null) {
			serviceBuilder.ConfigureServices(services => ConfigureMongoDbOptions(services, sectionName, connectionStringName));

			return this;
		}

		public IMongoDbWebhookStorageBuilder UseSubscriptionStore<TStore>()
			where TStore : MongoDbWebhookSubscriptionStrore {
			serviceBuilder.ConfigureServices(services => AddSubscriptionFactory(services));
			serviceBuilder.Use<IWebhookSubscriptionStore, TStore>();
			serviceBuilder.Use<IWebhookSubscriptionStore<IWebhookSubscription>, TStore>();
			serviceBuilder.Use<IWebhookSubscriptionStore<MongoDbWebhookSubscription>, TStore>();

			return this;
		}

		public IMongoDbWebhookStorageBuilder UseSubscriptionStoreProvider<TProvider>() 
			where TProvider : MongoDbWebhookSubscriptionStoreProvider {
			serviceBuilder.ConfigureServices(services => AddSubscriptionFactory(services));
			serviceBuilder.Use<IWebhookSubscriptionStoreProvider, TProvider>();
			serviceBuilder.Use<IWebhookSubscriptionStoreProvider<IWebhookSubscription>, TProvider>();
			serviceBuilder.Use<IWebhookSubscriptionStoreProvider<MongoDbWebhookSubscription>, TProvider>();

			return this;
		}
	}
}
