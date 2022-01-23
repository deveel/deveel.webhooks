﻿using System;
using System.Collections.Generic;

using Deveel.Data;

using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

using Xunit;

namespace Deveel.Webhooks.Storage {
	public static class MongoDbConfigurationTests {
		[Fact]
		public static void BuilderConfiguration() {
			var provider = new ServiceCollection()
				.AddWebhooks(webhooks => {
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
			Assert.Equal(MongoDbMultiTenancyHandling.TenantField, mongoDbOptions.MultiTenantHandling);
			Assert.Equal("TenantId", mongoDbOptions.TenantField);
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
				.AddWebhooks(webhooks => {
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
			Assert.Equal(MongoDbMultiTenancyHandling.TenantField, mongoDbOptions.MultiTenantHandling);
			Assert.Equal("TenantId", mongoDbOptions.TenantField);
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
				.AddWebhooks(webhooks => {
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
			Assert.Equal(MongoDbMultiTenancyHandling.TenantField, mongoDbOptions.MultiTenantHandling);
			Assert.Equal("TenantId", mongoDbOptions.TenantField);
		}

		[Fact]
		public static void ConfigureCustomStorage() {
			var provider = new ServiceCollection()
				.AddWebhooks(webhook => webhook.UseMongoDb(builder =>
					builder.Configure(options => {
						options.ConnectionString = "mongodb://127.0.0.1:2709";
						options.DatabaseName = "test";
					})
					.UseSubscriptionStore<MyMongoDbWebhookSubscriptionStore>()
					.UseSubscriptionStoreProvider<MyMongoDbWebhookSubscriptionStoreProvider>()))
				.BuildServiceProvider();

			Assert.NotNull(provider.GetService<IWebhookSubscriptionStore>());
			Assert.NotNull(provider.GetService<IWebhookSubscriptionStore<MongoDbWebhookSubscription>>());
			Assert.NotNull(provider.GetService<IWebhookSubscriptionStore<IWebhookSubscription>>());

			Assert.NotNull(provider.GetService<IWebhookSubscriptionStoreProvider>());
			Assert.NotNull(provider.GetService<IWebhookSubscriptionStoreProvider<MongoDbWebhookSubscription>>());
			Assert.NotNull(provider.GetService<IWebhookSubscriptionStoreProvider<IWebhookSubscription>>());
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
