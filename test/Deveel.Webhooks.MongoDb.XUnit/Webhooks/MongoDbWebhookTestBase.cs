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

using System.Net;

using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

using MongoDB.Driver;

using Xunit.Abstractions;

namespace Deveel.Webhooks {
	[Collection(nameof(MongoTestCollection))]
	public abstract class MongoDbWebhookTestBase : IAsyncLifetime {
		private readonly MongoTestDatabase mongo;

		protected MongoDbWebhookTestBase(MongoTestDatabase mongo, ITestOutputHelper outputHelper) {
			this.mongo = mongo;

			Client = new MongoClient(mongo.ConnectionString);

			Services = BuildServiceProvider(outputHelper);
			Scope = Services.CreateScope();
		}

		protected IServiceProvider Services { get; }

		protected IServiceScope Scope { get; }

		protected string ConnectionString => mongo.ConnectionString;

		protected MongoClient Client { get; }

		public virtual async Task InitializeAsync() {
			await Client.GetDatabase("webhooks").CreateCollectionAsync(MongoDbWebhookStorageConstants.SubscriptionCollectionName);
			await Client.GetDatabase("webhooks").CreateCollectionAsync(MongoDbWebhookStorageConstants.DeliveryResultsCollectionName);
		}

		public virtual async Task DisposeAsync() {
			await Client.GetDatabase("webhooks").DropCollectionAsync(MongoDbWebhookStorageConstants.SubscriptionCollectionName);
			await Client.GetDatabase("webhooks").DropCollectionAsync(MongoDbWebhookStorageConstants.DeliveryResultsCollectionName);

			Scope.Dispose();
		}

		private IServiceProvider BuildServiceProvider(ITestOutputHelper outputHelper) {
			var services = new ServiceCollection()
				.AddWebhookSubscriptions<MongoWebhookSubscription>(buidler => ConfigureWebhookService(buidler))
				.AddHttpCallback(OnRequestAsync)
				.AddLogging(logging => logging.AddXUnit(outputHelper));

			ConfigureServices(services);

			return services.BuildServiceProvider();
		}

		protected virtual Task<HttpResponseMessage> OnRequestAsync(HttpRequestMessage httpRequest) {
			return Task.FromResult(new HttpResponseMessage(HttpStatusCode.OK));
		}

		protected virtual void ConfigureWebhookService(WebhookSubscriptionBuilder<MongoWebhookSubscription> builder) {
			builder.UseMongoDb(options => {
				options.WithConnectionString($"{ConnectionString}webhooks");
			});
		}

		protected virtual void ConfigureServices(IServiceCollection services) {

		}
	}
}
