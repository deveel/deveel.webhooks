// Copyright 2022-2023 Deveel
// 
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
// 
//     http://www.apache.org/licenses/LICENSE-2.0
// 
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.

using Deveel.Data;

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
				.AddWebhookSubscriptions<MongoWebhookSubscription>(webhooks => {
					webhooks.UseMongoDb(mongo => mongo.WithConnectionStringName("MongoDb"));
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
				.AddWebhookSubscriptions<MongoWebhookSubscription>(webhook => webhook
					.UseMongoDb(builder => {
						builder.WithConnectionString("mongodb://127.0.0.1:2709/my_db");
						builder.UseSubscriptionRepository<MyMongoDbWebhookSubscriptionStore>();
					}))
				.BuildServiceProvider();

			var store = provider.GetService<IWebhookSubscriptionRepository<MongoWebhookSubscription>>();
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
