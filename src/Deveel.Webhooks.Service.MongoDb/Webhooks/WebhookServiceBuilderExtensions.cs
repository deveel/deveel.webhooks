using System;

using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Deveel.Webhooks {
	public static class WebhookServiceBuilderExtensions {
		private static IServiceCollection ConfigureMongoDbOptions(this IServiceCollection services, string sectionName, string connectionStringName = null) {
			services.AddOptions<MongoDbWebhookOptions>()
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

		private static IWebhookServiceBuilder UseMongoDb(this IWebhookServiceBuilder builder) {
			return builder
				.UseSubscriptionFactory<MongoDbWebhookSubscriptionFactory>()
				.UseSubscriptionStore<MongoDbWebhookSubscriptionStrore>()
				.UseSubscriptionStoreProvider<MongoDbWebhookSubscriptionStoreProvider>();
		}

		public static IWebhookServiceBuilder UseMongoDb(this IWebhookServiceBuilder builder, string sectionName, string connectionStringName = null) {
			builder.ConfigureServices(services => services.ConfigureMongoDbOptions(sectionName, connectionStringName));
			builder.UseMongoDb();
			return builder;
		}


		public static IWebhookServiceBuilder UseMongoDb(this IWebhookServiceBuilder builder, Action<MongoDbWebhookOptions> configure) {
			builder.ConfigureServices(services => services.AddOptions<MongoDbWebhookOptions>().Configure(configure));

			builder.UseMongoDb();
			return builder;
		}

		public static IWebhookServiceBuilder UseMongoDb(this IWebhookServiceBuilder builder, Action<IMongoDbOptionBuilder> configure) {
			builder.ConfigureServices(services => {
				services.AddOptions<MongoDbWebhookOptions>()
				.Configure(options => {
					var optionsBuilder = new MongoDbWebhookOptionBuilder(options);
					configure(optionsBuilder);
				});
			});

			return builder;
		}
	}
}
