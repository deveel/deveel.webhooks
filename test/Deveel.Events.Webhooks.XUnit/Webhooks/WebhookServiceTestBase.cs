﻿using System;
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

		private IServiceProvider BuildServiceProvider(ITestOutputHelper outputHelper) {
			mongoDbCluster = MongoDbRunner.Start(logger: NullLogger.Instance);

			return new ServiceCollection()
				.AddWebhooks<MongoDbWebhookSubscription>(buidler => {
				buidler.ConfigureDelivery(options =>
					options.SignWebhooks());

				ConfigureWebhookService(buidler);
			})
				.AddDynamicLinqFilterEvaluator()
				.AddTestHttpClient(OnRequestAsync)
				.AddLogging(logging => logging.AddXUnit(outputHelper))
				.BuildServiceProvider();
		}

		protected virtual Task<HttpResponseMessage> OnRequestAsync(HttpRequestMessage httpRequest) {
			return Task.FromResult(new HttpResponseMessage(HttpStatusCode.OK));
		}

		protected virtual void ConfigureWebhookService(WebhookServiceBuilder<MongoDbWebhookSubscription> builder) {
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
