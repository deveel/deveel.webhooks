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
using System.Net.Http.Json;
using System.Text;
using System.Web;
using System.Xml.Serialization;

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

			Func<HttpRequestMessage, Task<TestWebhook>> readContent = async request => {
				TestWebhook? webhook;

				if (request.Content!.Headers!.ContentType!.MediaType == "application/json") {
					webhook = await request.Content!.ReadFromJsonAsync<TestWebhook>();
				} else if (request.Content.Headers.ContentType.MediaType == "application/xml" ||
				           request.Content.Headers.ContentType.MediaType == "text/xml") {
					var xml = await request.Content!.ReadAsStringAsync();
					var serializer = new XmlSerializer(typeof(TestWebhook));
					webhook = (TestWebhook) serializer.Deserialize(new StringReader(xml))!;
				} else {
					throw new NotSupportedException($"The content type '{request.Content.Headers.ContentType.MediaType}' is not supported.");
				}

				return webhook!;
			};

			var mockHandler = new MockHttpMessageHandler();
			mockHandler.When(HttpMethod.Post, "http://localhost:8080/webhooks")
				.Respond(async request => {
					lastRequest = request;
					lastWebhook = await readContent(request);
					return new HttpResponseMessage(System.Net.HttpStatusCode.OK);
				});

			mockHandler.When(HttpMethod.Post, "http://localhost:8080/webhooks/timeout")
				.Respond(async request => {
                    lastRequest = request;
                    lastWebhook = await readContent(request);

					return new HttpResponseMessage(HttpStatusCode.RequestTimeout);
                });

			mockHandler.When(HttpMethod.Post, "http://localhost:8081/webhooks")
				.Respond(async request => {
					lastRequest = request;
					lastWebhook = await readContent(request);
					await Task.Delay(TimeSpan.FromMilliseconds(retryTimeoutMs + 100));

					return new HttpResponseMessage(HttpStatusCode.OK);
				});

			mockHandler.When(HttpMethod.Get, "http://localhost:8080/webhooks/verify")
				.Respond(request => {
					var query = HttpUtility.ParseQueryString(request.RequestUri!.Query);
					var token = query["token"];
					var challenge = query["challenge"];

					var valid = token == receiverToken;

					var response = new HttpResponseMessage(valid ? HttpStatusCode.OK : HttpStatusCode.Unauthorized);

					if (valid && !String.IsNullOrWhiteSpace(challenge))
						response.Content = new StringContent(challenge, Encoding.UTF8, "text/plain");

					return response;
				});

			mockHandler.When(HttpMethod.Get, "http://localhost:8080/webhooks/verify/invalid_challenge")
				.Respond(request => {
					var query = HttpUtility.ParseQueryString(request.RequestUri!.Query);
					var token = query["token"];
					
					var valid = token == receiverToken;

					return new HttpResponseMessage(valid ? HttpStatusCode.OK : HttpStatusCode.Unauthorized) {
						Content = new StringContent(Guid.NewGuid().ToString("N"), Encoding.UTF8, "text/plain")
					};
				});

			mockHandler.When(HttpMethod.Get, "http://localhost:8080/webhooks/verify/no_challenge")
				.Respond(request => {
					var query = HttpUtility.ParseQueryString(request.RequestUri!.Query);
					var token = query["token"];

					var valid = token == receiverToken;

					return new HttpResponseMessage(valid ? HttpStatusCode.OK : HttpStatusCode.Unauthorized);
				});


			var services = new ServiceCollection()
				.AddSingleton<IHttpClientFactory>(new MockHttpClientFactory("", mockHandler.ToHttpClient()))
				.AddLogging(logging => logging.AddXUnit(outputHelper, options => options.Filter = (cat, level) => true)
					.SetMinimumLevel(LogLevel.Trace));

			services.AddWebhookSender<TestWebhook>(options => {
				options.DefaultHeaders = new Dictionary<string, string> {
						{"X-Test", "true"}
					};
				options.Retry.Timeout = TimeSpan.FromMilliseconds(retryTimeoutMs);
				options.Signature.Location = WebhookSignatureLocation.QueryString;
				options.Signature.AlgorithmQueryParameter = "sig_alg";
				options.Signature.QueryParameter = "sig";
				options.Verification.Challenge = true;
			});

			return services.BuildServiceProvider();
		}


		public void Dispose() {
            serviceScope?.Dispose();
        }

		private IWebhookSender<TWebhook> GetSender<TWebhook>() where TWebhook : class
			=> serviceScope.ServiceProvider.GetRequiredService<IWebhookSender<TWebhook>>();

		private IWebhookDestinationVerifier<TWebhook> GetVerifier<TWebhook>() where TWebhook : class
			=> serviceScope.ServiceProvider.GetRequiredService<IWebhookDestinationVerifier<TWebhook>>();

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
		public async Task SendWebhookAsXml() {
			var sender = GetSender<TestWebhook>();

			var webhook = new TestWebhook {
				Id = "123",
				Event = "test",
				TimeStamp = DateTimeOffset.Now
			};

			var destination = new WebhookDestination("http://localhost:8080/webhooks") {
				Format = WebhookFormat.Xml
			};

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

			Assert.NotNull(queryString["sig_alg"]);
			Assert.Equal("sha256", queryString["sig_alg"]);

			var alg = queryString["sig_alg"];
			var signature = queryString["sig"];

			Assert.NotNull(alg);

			var json = await lastRequest.Content!.ReadAsStringAsync();

			var expectedSignature = WebhookSignature.Create(alg, json, destination.Signature!.Secret!);

			Assert.Equal(expectedSignature, signature);
        }

		[Fact]
		public async Task Validate_ValidReceiver() {
            var sender = GetVerifier<TestWebhook>();

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

            var result = await sender.VerifyDestinationAsync(destination);

			Assert.True(result.Successful);
        }

		[Fact]
		public async Task Validate_InvalidReceiverToken() {
			var sender = GetVerifier<TestWebhook>();

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

			var result = await sender.VerifyDestinationAsync(destination);

			Assert.False(result.Successful);
			Assert.NotNull(result.StatusCode);
			Assert.Equal(401, result.StatusCode);
		}

		[Fact]
		public async Task Verify_InvalidReceiverAddress() {
			var sender = GetVerifier<TestWebhook>();

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

			var result = await sender.VerifyDestinationAsync(destination);

			Assert.False(result.Successful);
			Assert.NotNull(result.StatusCode);
			Assert.Equal(404, result.StatusCode);
		}

		[Fact]
		public async Task Verify_InvalidChallengeReturned() {
			var sender = GetVerifier<TestWebhook>();

			var webhook = new TestWebhook {
				Id = "123",
				Event = "test",
				TimeStamp = DateTimeOffset.Now
			};

			var destination = new WebhookDestination("http://localhost:8080/webhooks")
				.WithVerification(options => {
					options.VerificationUrl = new Uri("http://localhost:8080/webhooks/verify/invalid_challenge");
					options.Parameters = new Dictionary<string, object> {
						{ "token", receiverToken }
					};
				});

			var result = await sender.VerifyDestinationAsync(destination);

			Assert.False(result.Successful);
			Assert.NotNull(result.StatusCode);
			Assert.Equal(401, result.StatusCode);
		}

		[Fact]
		public async Task Verify_NoChallengeReturned() {
			var sender = GetVerifier<TestWebhook>();

			var webhook = new TestWebhook {
				Id = "123",
				Event = "test",
				TimeStamp = DateTimeOffset.Now
			};

			var destination = new WebhookDestination("http://localhost:8080/webhooks")
				.WithVerification(options => {
					options.VerificationUrl = new Uri("http://localhost:8080/webhooks/verify/no_challenge");
					options.Parameters = new Dictionary<string, object> {
						{ "token", receiverToken }
					};
				});

			var result = await sender.VerifyDestinationAsync(destination);

			Assert.False(result.Successful);
			Assert.NotNull(result.StatusCode);
			Assert.Equal(400, result.StatusCode);
		}
	}
}
