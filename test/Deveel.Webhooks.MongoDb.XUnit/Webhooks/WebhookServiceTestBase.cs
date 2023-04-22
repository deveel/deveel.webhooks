using System;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;

using Deveel.Util;

using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;

using Mongo2Go;

using Xunit.Abstractions;

namespace Deveel.Webhooks {
	public abstract class WebhookServiceTestBase : IDisposable {
		private MongoDbRunner mongoDbCluster;

		protected WebhookServiceTestBase(ITestOutputHelper outputHelper) {
			Services = BuildServiceProvider(outputHelper);
		}

		protected IServiceProvider Services { get; }

		protected string ConnectionString => mongoDbCluster?.ConnectionString;

		protected virtual MongoDbRunner? CreateMongo() {
			return MongoDbRunner.Start(logger: NullLogger.Instance);
		}

		private IServiceProvider BuildServiceProvider(ITestOutputHelper outputHelper) {
			mongoDbCluster = CreateMongo();

			return new ServiceCollection()
				.AddWebhookSubscriptions<MongoDbWebhookSubscription>(buidler => ConfigureWebhookService(buidler))
				.AddTestHttpClient(OnRequestAsync)
				.AddLogging(logging => logging.AddXUnit(outputHelper))
				.BuildServiceProvider();
		}

		protected virtual Task<HttpResponseMessage> OnRequestAsync(HttpRequestMessage httpRequest) {
			return Task.FromResult(new HttpResponseMessage(HttpStatusCode.OK));
		}

		protected virtual void ConfigureWebhookService(WebhookSubscriptionBuilder<MongoDbWebhookSubscription> builder) {
			if (mongoDbCluster != null)
				builder.UseMongoDb(options => {
					options.DatabaseName = "webhooks";
					options.ConnectionString = mongoDbCluster.ConnectionString;
					options.SubscriptionsCollectionName("webhooks_subscription");
				});
		}

		protected virtual void ConfigureServices(IServiceCollection services) {

		}

		public virtual void Dispose() {
			mongoDbCluster?.Dispose();
		}
	}
}
