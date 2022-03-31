using System;
using System.Collections.Generic;

using Deveel.Data;
using Deveel.Webhooks;

using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

using Xunit;

namespace Deveel.Webhooks.Storage {
	public static class MongoDbConfigurationTests {
		[Fact]
		public static void BuilderConfiguration() {
			var provider = new ServiceCollection()
				.AddWebhooks<MongoDbWebhookSubscription>(webhooks => {
					webhooks.UseMongoDb(mongo => mongo
						.SetConnectionString("mongodb://127.0.0.1:2709")
						.SetDatabaseName("test")
						.SetSubscriptionsCollection("webhook_subscriptions"));
				})
				.BuildServiceProvider();

			var options = provider.GetService<IOptions<MongoDbOptions>>();

			Assert.NotNull(options);

			var mongoDbOptions = options.Value;

			Assert.NotNull(mongoDbOptions);
			Assert.Equal("mongodb://127.0.0.1:2709", mongoDbOptions.ConnectionString);
			Assert.Equal("test", mongoDbOptions.DatabaseName);
			Assert.Equal("webhook_subscriptions", mongoDbOptions.SubscriptionsCollectionName());
			Assert.NotNull(mongoDbOptions.MultiTenancy);
			Assert.Equal(MongoDbMultiTenancyHandling.TenantDatabase, mongoDbOptions.MultiTenancy.Handling);
			Assert.Equal("TenantId", mongoDbOptions.MultiTenancy.TenantField);
		}

		[Fact]
		public static void ConfigurationPattern() {
			var config = new ConfigurationBuilder()
				.AddInMemoryCollection(new Dictionary<string, string> {
					{ "MongoDb:Webhooks:ConnectionString", "mongodb://127.0.0.1:2709" },
					{ "MongoDb:Webhooks:DatabaseName", "test" },
					{ "MongoDb:Webhooks:Collections:WebhookSubscriptions", "webhook_subscriptions" }
				});

			var provider = new ServiceCollection()
				.AddSingleton<IConfiguration>(config.Build())
				.AddWebhooks<MongoDbWebhookSubscription>(webhooks => {
					webhooks.UseMongoDb("MongoDb:Webhooks");
				})
				.BuildServiceProvider();

			var options = provider.GetService<IOptions<MongoDbOptions>>();

			Assert.NotNull(options);

			var mongoDbOptions = options.Value;

			Assert.NotNull(mongoDbOptions);
			Assert.Equal("mongodb://127.0.0.1:2709", mongoDbOptions.ConnectionString);
			Assert.Equal("test", mongoDbOptions.DatabaseName);
			Assert.Equal("webhook_subscriptions", mongoDbOptions.SubscriptionsCollectionName());
			Assert.NotNull(mongoDbOptions.MultiTenancy);
			Assert.Equal(MongoDbMultiTenancyHandling.TenantDatabase, mongoDbOptions.MultiTenancy.Handling);
			Assert.Equal("TenantId", mongoDbOptions.MultiTenancy.TenantField);
		}

		[Fact]
		public static void ConfigurationPattern_ExternalConnectionString() {
			var config = new ConfigurationBuilder()
				.AddInMemoryCollection(new Dictionary<string, string> {
					{ "ConnectionStrings:MongoDb", "mongodb://127.0.0.1:2709" },
					{ "MongoDb:Webhooks:DatabaseName", "test" },
					{ "MongoDb:Webhooks:Collections:WebhookSubscriptions", "webhook_subscriptions" }
				});

			var provider = new ServiceCollection()
				.AddSingleton<IConfiguration>(config.Build())
				.AddWebhooks<MongoDbWebhookSubscription>(webhooks => {
					webhooks.UseMongoDb("MongoDb:Webhooks", "MongoDb");
				})
				.BuildServiceProvider();

			var options = provider.GetService<IOptions<MongoDbOptions>>();

			Assert.NotNull(options);

			var mongoDbOptions = options.Value;

			Assert.NotNull(mongoDbOptions);
			Assert.Equal("mongodb://127.0.0.1:2709", mongoDbOptions.ConnectionString);
			Assert.Equal("test", mongoDbOptions.DatabaseName);
			Assert.Equal("webhook_subscriptions", mongoDbOptions.SubscriptionsCollectionName());
			Assert.NotNull(mongoDbOptions.MultiTenancy);
			Assert.Equal(MongoDbMultiTenancyHandling.TenantDatabase, mongoDbOptions.MultiTenancy.Handling);
			Assert.Equal("TenantId", mongoDbOptions.MultiTenancy.TenantField);
		}

		[Fact]
		public static void ConfigureCustomStorage() {
			var provider = new ServiceCollection()
				.AddWebhooks<MongoDbWebhookSubscription>(webhook => webhook.UseMongoDb(builder =>
					builder.Configure(options => {
						options.ConnectionString = "mongodb://127.0.0.1:2709";
						options.DatabaseName = "test";
					})
					.UseSubscriptionStore<MyMongoDbWebhookSubscriptionStore>()
					.UseSubscriptionStoreProvider<MyMongoDbWebhookSubscriptionStoreProvider>()))
				.BuildServiceProvider();

			Assert.NotNull(provider.GetService<IWebhookSubscriptionStore<MongoDbWebhookSubscription>>());
			Assert.NotNull(provider.GetService<IWebhookSubscriptionStoreProvider<MongoDbWebhookSubscription>>());
		}

		#region MyMongoDbWebhookSubscriptionStore

		class MyMongoDbWebhookSubscriptionStore : MongoDbWebhookSubscriptionStrore {
			public MyMongoDbWebhookSubscriptionStore(IOptions<MongoDbOptions> options) 
				: base(options) {
			}
		}

		#endregion

		#region MyMongoDbWebhookSubscriptionStoreProvider

		class MyMongoDbWebhookSubscriptionStoreProvider : MongoDbWebhookSubscriptionStoreProvider {
			public MyMongoDbWebhookSubscriptionStoreProvider(IOptions<MongoDbOptions> baseOptions) : base(baseOptions) {
			}
		}

		#endregion
	}
}
