using System;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;

using Deveel.Util;

using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

using Xunit.Abstractions;

namespace Deveel.Webhooks {
	public abstract class WebhookServiceTestBase {
		protected WebhookServiceTestBase(ITestOutputHelper outputHelper) {
			Services = BuildServiceProvider(outputHelper);
		}

		protected IServiceProvider Services { get; }

		private IServiceProvider BuildServiceProvider(ITestOutputHelper outputHelper) {
			return new ServiceCollection()
				.AddWebhookSubscriptions<TestWebhookSubscription>(buidler => ConfigureWebhookService(buidler))
				.AddTestHttpClient(OnRequestAsync)
				.AddLogging(logging => logging.AddXUnit(outputHelper))
				.BuildServiceProvider();
		}

		protected virtual Task<HttpResponseMessage> OnRequestAsync(HttpRequestMessage httpRequest) {
			return Task.FromResult(new HttpResponseMessage(HttpStatusCode.OK));
		}

		protected virtual void ConfigureWebhookService(WebhookSubscriptionBuilder<TestWebhookSubscription> builder) {
		}

		protected virtual void ConfigureServices(IServiceCollection services) {

		}
	}
}
