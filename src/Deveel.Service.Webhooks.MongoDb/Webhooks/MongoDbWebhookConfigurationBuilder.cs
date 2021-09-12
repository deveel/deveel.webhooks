using System;

using Deveel.Data;

using Microsoft.Extensions.Configuration;

namespace Deveel.Webhooks {
	public static class MongoDbWebhookConfigurationBuilder {
		private static IWebhookServiceBuilder UseMongoDb(this IWebhookServiceBuilder builder) {
			return builder
				.UseSubscriptionFactory<MongoDbWebhookSubscriptionFactory>()
				.UseSubscriptionStore<MongoDbWebhookSubscriptionStrore>()
				.UseSubscriptionStoreProvider<MongoDbWebhookSubscriptionStoreProvider>();
		}

		public static IWebhookServiceBuilder UseMongoDbStorage(this IWebhookServiceBuilder builder, IConfiguration configuration, string sectionName, string connectionStringName = null) {
			builder.ConfigureServices(services => services.ConfigureMongoDbOptions<WebhookSubscriptionDocument>(configuration, sectionName, connectionStringName));
			builder.UseMongoDb();
			return builder;
		}

		public static IWebhookServiceBuilder UseMongoDbStorage(this IWebhookServiceBuilder builder, string sectionName, string connectionStringName = null) {
			builder.ConfigureServices(services => services.ConfigureMongoDbOptions<WebhookSubscriptionDocument>(sectionName, connectionStringName));
			builder.UseMongoDb();
			return builder;
		}


		public static IWebhookServiceBuilder UseMongoDbStorage(this IWebhookServiceBuilder builder, Action<MongoDbOptions> options) {
			builder.ConfigureServices(services => services.ConfigureMongoDbOptions<WebhookSubscriptionDocument>(options));
			builder.UseMongoDb();
			return builder;
		}

		public static IWebhookServiceBuilder UseMongoDbStorage(this IWebhookServiceBuilder builder, MongoDbOptions options) {
			builder.ConfigureServices(services => services.AddMongoDbOptions<WebhookSubscriptionDocument>(options));
			return builder.UseMongoDb();
		}
	}
}
