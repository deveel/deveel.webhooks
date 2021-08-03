using System;

using Deveel.Data;
using Deveel.Webhooks;

using Microsoft.Extensions.Configuration;

namespace Deveel.Events {
	public static class MongoDbWebhookConfigurationBuilder {
		private static IWebhookConfigurationBuilder UseMongoDb(this IWebhookConfigurationBuilder builder) {
			return builder.UseSubscriptionFactory<MongoDbWebhookSubscriptionFactory>()
				.UseSubscriptionStore<MongoDbWebhookSubscriptionStrore>()
				.UseSubscriptionStoreProvider<MongoDbWebhookSubscriptionStoreProvider>();
		}

		public static IWebhookConfigurationBuilder AddMongoDb(this IWebhookConfigurationBuilder builder, IConfiguration configuration, string sectionName, string connectionStringName = null) {
			builder.Services.ConfigureMongoDbOptions<WebhookSubscriptionDocument>(configuration, sectionName, connectionStringName);
			builder.UseMongoDb();

			// builder.Services.AddMongoDbWebhooks(configuration, sectionName, connectionStringName);
			return builder;
		}

		public static IWebhookConfigurationBuilder AddMongoDb(this IWebhookConfigurationBuilder builder, Action<MongoDbOptions> options) {
			builder.Services.ConfigureMongoDbOptions<WebhookSubscriptionDocument>(options);
			builder.UseMongoDb();
			return builder;
		}
	}
}
