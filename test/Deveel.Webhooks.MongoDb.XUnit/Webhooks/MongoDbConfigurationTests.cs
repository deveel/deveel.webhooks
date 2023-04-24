using Finbuckle.MultiTenant;

using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

using MongoFramework;

namespace Deveel.Webhooks {
	public static class MongoDbConfigurationTests {
		[Fact]
		public static void WithConnection() {
			var provider = new ServiceCollection()
				.AddWebhookSubscriptions<MongoWebhookSubscription>(webhooks =>
					webhooks.UseMongoDb("mongodb://127.0.0.1:2709/my_db"))
				.BuildServiceProvider();

			var context = provider.GetService<IMongoDbWebhookContext>();
			Assert.NotNull(context);
			Assert.IsNotType<MongoDbWebhookTenantContext<TenantInfo>>(context);
			Assert.NotNull(context.Connection);
			
			var dbConnection = Assert.IsType<MongoDbConnection>(context.Connection);
			Assert.Equal("mongodb://127.0.0.1:2709/my_db", dbConnection.Url.ToString());
		}

		[Fact]
		public static void ConfigurationPattern() {
			var config = new ConfigurationBuilder()
				.AddInMemoryCollection(new Dictionary<string, string> {
					{ "MongoDb:Webhooks:ConnectionString", "mongodb://127.0.0.1:2709/test" },
				});

			var provider = new ServiceCollection()
				.AddSingleton<IConfiguration>(config.Build())
				.AddWebhookSubscriptions<MongoWebhookSubscription>(webhooks => {
					webhooks.UseMongoDb(mongo => mongo.Configure("MongoDb:Webhooks"));
				})
				.BuildServiceProvider();

			var context = provider.GetService<IMongoDbWebhookContext>();
			Assert.NotNull(context);
			Assert.IsNotType<MongoDbWebhookTenantContext<TenantInfo>>(context);
			Assert.NotNull(context.Connection);

			var dbConnection = Assert.IsType<MongoDbConnection>(context.Connection);

			Assert.Equal("mongodb://127.0.0.1:2709/test", dbConnection.Url.ToString());
		}

		[Fact]
		public static void ConfigurationPattern_ExternalConnectionString() {
			var config = new ConfigurationBuilder()
				.AddInMemoryCollection(new Dictionary<string, string> {
					{ "ConnectionStrings:MongoDb", "mongodb://127.0.0.1:2709/my_db" }
				});

			var provider = new ServiceCollection()
				.AddSingleton<IConfiguration>(config.Build())
				.AddWebhookSubscriptions<MongoWebhookSubscription>(webhooks => {
					webhooks.UseMongoDb(mongo => mongo.WithConnectionStringName("MongoDb"));
				})
				.BuildServiceProvider();

			var context = provider.GetService<IMongoDbWebhookContext>();
			Assert.NotNull(context);
			Assert.IsNotType<MongoDbWebhookTenantContext<TenantInfo>>(context);
			Assert.NotNull(context.Connection);

			var dbConnection = Assert.IsType<MongoDbConnection>(context.Connection);

			Assert.Equal("mongodb://127.0.0.1:2709/my_db", dbConnection.Url.ToString());
		}

		[Fact]
		public static void ConfigureCustomStorage() {
			var provider = new ServiceCollection()
				.AddWebhookSubscriptions<MongoWebhookSubscription>(webhook => webhook
					.UseMongoDb(builder => {
						builder.WithConnectionString("mongodb://127.0.0.1:2709/my_db");
						builder.UseSubscriptionStore<MyMongoDbWebhookSubscriptionStore>();
					}))
				.BuildServiceProvider();

			var store = provider.GetService<IWebhookSubscriptionStore<MongoWebhookSubscription>>();
			Assert.NotNull(store);
			Assert.IsType<MyMongoDbWebhookSubscriptionStore>(store);
		}

		#region MyMongoDbWebhookSubscriptionStore

		class MyMongoDbWebhookSubscriptionStore : MongoDbWebhookSubscriptionStrore {
			public MyMongoDbWebhookSubscriptionStore(IMongoDbWebhookContext context) : base(context) {
			}
		}

		#endregion
	}
}
