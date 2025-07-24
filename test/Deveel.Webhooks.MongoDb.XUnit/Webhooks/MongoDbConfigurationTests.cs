using Deveel.Data;

using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

using MongoDB.Bson;

namespace Deveel.Webhooks {
	public static class MongoDbConfigurationTests {
		[Fact]
		public static void WithConnection() {
			var provider = new ServiceCollection()
				.AddWebhookSubscriptions<MongoWebhookSubscription, ObjectId>(webhooks =>
					webhooks.UseMongoDb("mongodb://127.0.0.1:2709/my_db"))
				.BuildServiceProvider();

			var context = provider.GetService<MongoDbWebhookContext>();
			Assert.NotNull(context);
			Assert.IsNotType<MongoDbWebhookTenantContext>(context);
			Assert.NotNull(context.Connection);
			
			var dbConnection = Assert.IsType<MongoDbConnection<MongoDbWebhookContext>>(context.Connection);
			Assert.Equal("mongodb://127.0.0.1:2709/my_db", dbConnection.Url!.ToString());
		}

		[Fact]
		public static void ConfigurationPattern_ExternalConnectionString() {
			var config = new ConfigurationBuilder()
				.AddInMemoryCollection(new Dictionary<string, string?> {
					{ "ConnectionStrings:MongoDb", "mongodb://127.0.0.1:2709/my_db" }
				});

			var provider = new ServiceCollection()
				.AddSingleton<IConfiguration>(config.Build())
				.AddWebhookSubscriptions<MongoWebhookSubscription, ObjectId>(webhooks => {
					webhooks.UseMongoDb(mongo => mongo.WithConnectionStringName(config.Build(), "MongoDb"));
				})
				.BuildServiceProvider();

			var context = provider.GetService<MongoDbWebhookContext>();
			Assert.NotNull(context);
			Assert.IsNotType<MongoDbWebhookTenantContext>(context);
			Assert.NotNull(context.Connection);

			var dbConnection = Assert.IsType<MongoDbConnection<MongoDbWebhookContext>>(context.Connection);

			Assert.NotNull(dbConnection.Url);
			Assert.Equal("mongodb://127.0.0.1:2709/my_db", dbConnection.Url.ToString());
		}

		[Fact]
		public static void ConfigureCustomStorage() {
			var provider = new ServiceCollection()
				.AddWebhookSubscriptions<MongoWebhookSubscription, ObjectId>(webhook => webhook
					.UseMongoDb(builder => {
						builder.WithConnectionString("mongodb://127.0.0.1:2709/my_db");
						builder.UseSubscriptionRepository<MyMongoDbWebhookSubscriptionStore>();
					}))
				.BuildServiceProvider();

			var store = provider.GetService<IWebhookSubscriptionRepository<MongoWebhookSubscription, ObjectId>>();
			Assert.NotNull(store);
			Assert.IsType<MyMongoDbWebhookSubscriptionStore>(store);
		}

		#region MyMongoDbWebhookSubscriptionStore

		class MyMongoDbWebhookSubscriptionStore : MongoDbWebhookSubscriptionRepository {
			public MyMongoDbWebhookSubscriptionStore(MongoDbWebhookContext context) : base(context) {
			}
		}

		#endregion
	}
}
