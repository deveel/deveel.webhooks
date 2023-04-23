using System.Net;

using Deveel.Util;

using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;

using Mongo2Go;

using MongoDB.Driver;

using Xunit.Abstractions;

namespace Deveel.Webhooks {
	[Collection(nameof(MongoTestCollection))]
	public abstract class WebhookServiceTestBase : IAsyncLifetime {
		private readonly MongoTestCluster mongo;

		protected WebhookServiceTestBase(MongoTestCluster mongo, ITestOutputHelper outputHelper) {
			this.mongo = mongo;
			Services = BuildServiceProvider(outputHelper);
		}

		protected IServiceProvider Services { get; }

		protected string ConnectionString => mongo.ConnectionString;

		protected virtual MongoDbRunner? CreateMongo() {
			return MongoDbRunner.Start(logger: NullLogger.Instance);
		}

		public virtual async Task InitializeAsync() {
			var client = new MongoClient(ConnectionString);
			await client.GetDatabase("webhooks").CreateCollectionAsync(MongoDbWebhookStorageConstants.SubscriptionCollectionName);
			await client.GetDatabase("webhooks").CreateCollectionAsync(MongoDbWebhookStorageConstants.DeliveryResultsCollectionName);
		}

		public virtual async Task DisposeAsync() {
			var client = new MongoClient(ConnectionString);

			await client.GetDatabase("webhooks").DropCollectionAsync(MongoDbWebhookStorageConstants.SubscriptionCollectionName);
			await client.GetDatabase("webhooks").DropCollectionAsync(MongoDbWebhookStorageConstants.DeliveryResultsCollectionName);
		}

		private IServiceProvider BuildServiceProvider(ITestOutputHelper outputHelper) {
			return new ServiceCollection()
				.AddWebhookSubscriptions<MongoWebhookSubscription>(buidler => ConfigureWebhookService(buidler))
				.AddTestHttpClient(OnRequestAsync)
				.AddLogging(logging => logging.AddXUnit(outputHelper))
				.BuildServiceProvider();
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
