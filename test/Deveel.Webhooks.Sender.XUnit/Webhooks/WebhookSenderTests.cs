using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Json;
using System.Text;
using System.Threading.Tasks;

using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

using RichardSzalay.MockHttp;

using Xunit.Abstractions;

namespace Deveel.Webhooks {
	public class WebhookSenderTests {
		private readonly IServiceProvider serviceProvider;
		private HttpRequestMessage? lastRequest;
		private TestWebhook? lastWebhook;

		public WebhookSenderTests(ITestOutputHelper outputHelper) {
			serviceProvider = ConfigureServices(outputHelper);
		}

		private IServiceProvider ConfigureServices(ITestOutputHelper outputHelper) {
			var mockRequest = new MockHttpMessageHandler();
			mockRequest.When(HttpMethod.Post, "http://localhost:8080/webhooks")
				.Respond(async request => {
					lastRequest = request;
					lastWebhook = await request.Content.ReadFromJsonAsync<TestWebhook>();
					return new HttpResponseMessage(System.Net.HttpStatusCode.OK);
				});

			var services = new ServiceCollection()
				.AddSingleton<IHttpClientFactory>(new MockHttpClientFactory("", mockRequest.ToHttpClient()))
				.AddLogging(logging => logging.AddXUnit(outputHelper).SetMinimumLevel(LogLevel.Trace));

			services.AddWebhookSender<TestWebhook>();

			return services.BuildServiceProvider();
		}

		[Fact]
		public async Task SendWebhook() {
			var sender = serviceProvider.GetRequiredService<IWebhookSender<TestWebhook>>();

			var webhook = new TestWebhook {
				Id = "123",
				Event = "test",
				TimeStamp = DateTimeOffset.Now
			};

			var destination = new WebhookDestination("http://localhost:8080/webhooks");
			var result = await sender.SendAsync(destination, webhook);

			Assert.NotNull(result);
			Assert.True(result.Successful);
			Assert.Equal(1, result.AttemptCount);

			Assert.NotNull(lastRequest);
			Assert.NotNull(lastWebhook);

			Assert.Equal(webhook.Id, lastWebhook!.Id);
			Assert.Equal(webhook.Event, lastWebhook!.Event);
			Assert.Equal(webhook.TimeStamp, lastWebhook!.TimeStamp);
		}

		class TestWebhook {
			public string Id { get; set; }

			public string Event { get; set; }

			public DateTimeOffset TimeStamp { get; set; }
		}
	}
}
