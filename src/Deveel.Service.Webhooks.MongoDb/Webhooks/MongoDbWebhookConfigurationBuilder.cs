using System;

using Deveel.Data;

using Microsoft.Extensions.Configuration;

namespace Deveel.Webhooks {
	public static class MongoDbWebhookConfigurationBuilder {
		private static IWebhookServiceBuilder UseMongoDb(this IWebhookServiceBuilder builder) {
			return builder.UseSubscriptionFactory<MongoDbWebhookSubscriptionFactory>()
				.UseSubscriptionStore<MongoDbWebhookSubscriptionStrore>()
				.UseSubscriptionStoreProvider<MongoDbWebhookSubscriptionStoreProvider>();
		}

		public static IWebhookServiceBuilder UseMongoDbStorage(this IWebhookServiceBuilder builder, IConfiguration configuration, string sectionName, string connectionStringName = null) {
			builder.Configure(services => services.ConfigureMongoDbOptions<WebhookSubscriptionDocument>(configuration, sectionName, connectionStringName));
			builder.UseMongoDb();

			// builder.Services.AddMongoDbWebhooks(configuration, sectionName, connectionStringName);
			return builder;
		}

		public static IWebhookServiceBuilder UseMongoDbStorage(this IWebhookServiceBuilder builder, Action<MongoDbOptions> options) {
			builder.Configure(services => services.ConfigureMongoDbOptions<WebhookSubscriptionDocument>(options));
			builder.UseMongoDb();
			return builder;
		}
	}
}
