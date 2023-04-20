using System.Net;
using System.Net.Http.Json;
using System.Web;

using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

using RichardSzalay.MockHttp;

using Xunit.Abstractions;

namespace Deveel.Webhooks {
	public class WebhookSenderTests : IDisposable {
		private readonly IServiceScope serviceScope;
		private HttpRequestMessage? lastRequest;
		private TestWebhook? lastWebhook;

		private string receiverToken;

		public WebhookSenderTests(ITestOutputHelper outputHelper) {
			var services = ConfigureServices(outputHelper);
			serviceScope = services.CreateScope();

			receiverToken = Guid.NewGuid().ToString("N");
		}

		private IServiceProvider ConfigureServices(ITestOutputHelper outputHelper) {
			var retryTimeoutMs = 500;

			var mockHandler = new MockHttpMessageHandler();
			mockHandler.When(HttpMethod.Post, "http://localhost:8080/webhooks")
				.Respond(async request => {
					lastRequest = request;
					lastWebhook = await request.Content.ReadFromJsonAsync<TestWebhook>();
					return new HttpResponseMessage(System.Net.HttpStatusCode.OK);
				});

			mockHandler.When(HttpMethod.Post, "http://localhost:8080/webhooks/timeout")
				.Respond(async request => {
                    lastRequest = request;
                    lastWebhook = await request.Content.ReadFromJsonAsync<TestWebhook>();

					return new HttpResponseMessage(HttpStatusCode.RequestTimeout);
                });

			mockHandler.When(HttpMethod.Post, "http://localhost:8081/webhooks")
				.Respond(async request => {
					lastRequest = request;
					lastWebhook = await request.Content.ReadFromJsonAsync<TestWebhook>();
					await Task.Delay(TimeSpan.FromMilliseconds(retryTimeoutMs + 100));

					return new HttpResponseMessage(HttpStatusCode.OK);
				});

			mockHandler.When(HttpMethod.Get, "http://localhost:8080/webhooks/verify")
				.Respond(request => {
					var query = HttpUtility.ParseQueryString(request.RequestUri!.Query);
					var token = query["token"];

					var valid = token == receiverToken;

					return new HttpResponseMessage(valid ? HttpStatusCode.OK : HttpStatusCode.Unauthorized);
				});

			var services = new ServiceCollection()
				.AddSingleton<IHttpClientFactory>(new MockHttpClientFactory("", mockHandler.ToHttpClient()))
				.AddLogging(logging => logging.AddXUnit(outputHelper).SetMinimumLevel(LogLevel.Trace));

			services.AddWebhookSender<TestWebhook>()
				.Configure(options => {
					options.DefaultHeaders = new Dictionary<string, string> {
						{"X-Test", "true"}
                    };
					options.Retry.Timeout = TimeSpan.FromMilliseconds(retryTimeoutMs);
					options.Signature.Location = WebhookSignatureLocation.QueryString;
					options.Signature.AlgorithmQueryParameter = "sig_alg";
					options.Signature.QueryParameter = "sig";
				});

			return services.BuildServiceProvider();
		}


		public void Dispose() {
            serviceScope?.Dispose();
        }

		private IWebhookSender<TWebhook> GetSender<TWebhook>() where TWebhook : class
			=> serviceScope.ServiceProvider.GetRequiredService<IWebhookSender<TWebhook>>();

		[Fact]
		public async Task SendWebhook() {
			var sender = GetSender<TestWebhook>();

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

        [Fact]
        public async Task SendWebhook_TimeoutError_NoRetries() {
            var sender = GetSender<TestWebhook>();

            var webhook = new TestWebhook {
                Id = "123",
                Event = "test",
                TimeStamp = DateTimeOffset.Now
            };

            var destination = new WebhookDestination("http://localhost:8081/webhooks");
            var result = await sender.SendAsync(destination, webhook);

            Assert.NotNull(result);
            Assert.False(result.Successful);
            Assert.Equal(1, result.AttemptCount); // The first attempt is not a retry
			Assert.Equal((int)HttpStatusCode.RequestTimeout, result.LastAttempt!.ResponseCode);

            Assert.NotNull(lastRequest);
            Assert.NotNull(lastWebhook);

            Assert.Equal(webhook.Id, lastWebhook!.Id);
            Assert.Equal(webhook.Event, lastWebhook!.Event);
            Assert.Equal(webhook.TimeStamp, lastWebhook!.TimeStamp);
        }

        [Fact]
        public async Task SendWebhook_TimeoutErrorRetried() {
            var sender = GetSender<TestWebhook>();

            var webhook = new TestWebhook {
                Id = "123",
                Event = "test",
                TimeStamp = DateTimeOffset.Now
            };

			var destination = new WebhookDestination("http://localhost:8081/webhooks")
				.WithRetry(options => options.MaxRetries = 3);

			var result = await sender.SendAsync(destination, webhook);

            Assert.NotNull(result);
            Assert.False(result.Successful);
            Assert.Equal(4, result.AttemptCount); // The first attempt is not a retry
            Assert.Equal((int)HttpStatusCode.RequestTimeout, result.LastAttempt!.ResponseCode);

            Assert.NotNull(lastRequest);
            Assert.NotNull(lastWebhook);

            Assert.Equal(webhook.Id, lastWebhook!.Id);
            Assert.Equal(webhook.Event, lastWebhook!.Event);
            Assert.Equal(webhook.TimeStamp, lastWebhook!.TimeStamp);
        }

        [Fact]
        public async Task SendWebhook_TimeoutResponse() {
            var sender = GetSender<TestWebhook>();

            var webhook = new TestWebhook {
                Id = "123",
                Event = "test",
                TimeStamp = DateTimeOffset.Now
            };

            var destination = new WebhookDestination("http://localhost:8080/webhooks/timeout");
            var result = await sender.SendAsync(destination, webhook);

            Assert.NotNull(result);
            Assert.False(result.Successful);
            Assert.Equal(1, result.AttemptCount); // The first attempt is not a retry
            Assert.Equal((int)HttpStatusCode.RequestTimeout, result.LastAttempt!.ResponseCode);

            Assert.NotNull(lastRequest);
            Assert.NotNull(lastWebhook);

            Assert.Equal(webhook.Id, lastWebhook!.Id);
            Assert.Equal(webhook.Event, lastWebhook!.Event);
            Assert.Equal(webhook.TimeStamp, lastWebhook!.TimeStamp);
        }



        [Fact]
		public async Task SendWebhookWithSignature() {
            var sender = GetSender<TestWebhook>();

            var webhook = new TestWebhook {
                Id = "123",
                Event = "test",
                TimeStamp = DateTimeOffset.Now
            };

			var destination = new WebhookDestination("http://localhost:8080/webhooks")
				.WithSignature(options => options.Secret = Guid.NewGuid().ToString());

            var result = await sender.SendAsync(destination, webhook);
            Assert.NotNull(result);
            Assert.True(result.Successful);
            Assert.Equal(1, result.AttemptCount);

            Assert.NotNull(lastRequest);
            Assert.NotNull(lastWebhook);

			var queryString = HttpUtility.ParseQueryString(lastRequest.RequestUri!.Query);

			Assert.True(queryString.HasKeys());
			Assert.Contains("sig_alg", queryString.AllKeys);
			Assert.Contains("sig", queryString.AllKeys);

			Assert.Equal("sha256", queryString["sig_alg"]);
			var alg = queryString["sig_alg"];
			var signature = queryString["sig"];

			var json = await lastRequest.Content!.ReadAsStringAsync();

			var expectedSignature = WebhookSignature.Create(alg, json, destination.Signature!.Secret);

			Assert.Equal(expectedSignature, signature);
        }

		[Fact]
		public async Task SendWebhook_ValidReceiver() {
            var sender = GetSender<TestWebhook>();

            var webhook = new TestWebhook {
                Id = "123",
                Event = "test",
                TimeStamp = DateTimeOffset.Now
            };

			var destination = new WebhookDestination("http://localhost:8080/webhooks")
				.WithVerification(options => {
					options.VerificationUrl = new Uri("http://localhost:8080/webhooks/verify");
					options.Parameters = new Dictionary<string, object> {
						{ "token", receiverToken }
					};
				});

            var result = await sender.SendAsync(destination, webhook);
            Assert.NotNull(result);
            Assert.True(result.Successful);
            Assert.Equal(1, result.AttemptCount);

            Assert.NotNull(lastRequest);
            Assert.NotNull(lastWebhook);
        }

		[Fact]
		public async Task SendWebhook_InvalidReceiverToken() {
			var sender = GetSender<TestWebhook>();

			var webhook = new TestWebhook {
				Id = "123",
				Event = "test",
				TimeStamp = DateTimeOffset.Now
			};

			var destination = new WebhookDestination("http://localhost:8080/webhooks")
				.WithVerification(options => {
					options.VerificationUrl = new Uri("http://localhost:8080/webhooks/verify");
					options.Parameters = new Dictionary<string, object> {
						{ "token", Guid.NewGuid().ToString("N") }
					};
				});

			var result = await Assert.ThrowsAsync<WebhookVerificationException>(() => sender.SendAsync(destination, webhook));

			Assert.NotNull(result);

			Assert.Null(lastRequest);
			Assert.Null(lastWebhook);
		}

		[Fact]
		public async Task SendWebhook_InvalidReceiverAddress() {
			var sender = GetSender<TestWebhook>();

			var webhook = new TestWebhook {
				Id = "123",
				Event = "test",
				TimeStamp = DateTimeOffset.Now
			};

			var destination = new WebhookDestination("http://localhost:8080/webhooks")
				.WithVerification(options => {
					options.VerificationUrl = new Uri("http://localhost:8083/webhooks/verify");
					options.Parameters = new Dictionary<string, object> {
						{ "token", receiverToken }
					};
				});

			var result = await Assert.ThrowsAsync<WebhookVerificationException>(() => sender.SendAsync(destination, webhook));

			Assert.NotNull(result);

			Assert.Null(lastRequest);
			Assert.Null(lastWebhook);
		}



		class TestWebhook {
			public string Id { get; set; }

			public string Event { get; set; }

			public DateTimeOffset TimeStamp { get; set; }
		}
	}
}
