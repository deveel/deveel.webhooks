using System;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

using Deveel.Webhooks.Handlers;
using Deveel.Webhooks.Model;
using Deveel.Webhooks.Receiver.TestApi;

using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

using Newtonsoft.Json;

using Xunit;
using Xunit.Abstractions;

namespace Deveel.Webhooks {
	public class WebhookReceiveRequestTests : IDisposable {
		private readonly WebApplicationFactory<Program> appFactory;
		private TestWebhook lastWebhook;

		public WebhookReceiveRequestTests(ITestOutputHelper outputHelper) {
			appFactory = new WebApplicationFactory<Program>()
				.WithWebHostBuilder(builder => builder
					.ConfigureTestServices(ConfigureServices)
					.ConfigureLogging(logging => logging.AddXUnit(outputHelper, opt => opt.Filter = (cat, level) => true)
						.SetMinimumLevel(LogLevel.Trace)));
		}

		private void ConfigureServices(IServiceCollection services) {
			services.AddWebhooks<TestWebhook>()
				.UseCallback(webhook => lastWebhook = webhook);

			services.AddWebhooks<TestSignedWebhook>()
				.UseCallback(webhook => lastWebhook = webhook);
		}

		public void Dispose() => appFactory?.Dispose();

		private HttpClient CreateClient() => appFactory.CreateClient();

		[Fact]
		public async Task ReceiveTestWebhook() {
			var client = CreateClient();

			var response = await client.SendAsync(new HttpRequestMessage(HttpMethod.Post, "/webhook") {
				Content = new StringContent(System.Text.Json.JsonSerializer.Serialize(new TestWebhook {
					Id = Guid.NewGuid().ToString("N"),
					Event = "test",
					TimeStamp = DateTimeOffset.Now,
				}), Encoding.UTF8, "application/json")
			});

			Assert.True(response.IsSuccessStatusCode);
			Assert.Equal(HttpStatusCode.NoContent, response.StatusCode);

			Assert.NotNull(lastWebhook);
			Assert.Equal("test", lastWebhook.Event);
		}

		[Fact]
		public async Task ReceiveHandledTestWebhook() {
			var client = CreateClient();

			var response = await client.SendAsync(new HttpRequestMessage(HttpMethod.Post, "/webhook/handled") {
				Content = new StringContent(System.Text.Json.JsonSerializer.Serialize(new TestWebhook {
					Id = Guid.NewGuid().ToString("N"),
					Event = "test",
					TimeStamp = DateTimeOffset.Now,
				}), Encoding.UTF8, "application/json")
			});

			Assert.True(response.IsSuccessStatusCode);
			Assert.Equal(HttpStatusCode.NoContent, response.StatusCode);

			Assert.NotNull(lastWebhook);
		}

		[Fact]
		public async Task ReceiveAsyncHandledTestWebhook() {
			var client = CreateClient();

			var response = await client.SendAsync(new HttpRequestMessage(HttpMethod.Post, "/webhook/handled/async") {
				Content = new StringContent(System.Text.Json.JsonSerializer.Serialize(new TestWebhook {
					Id = Guid.NewGuid().ToString("N"),
					Event = "test",
					TimeStamp = DateTimeOffset.Now,
				}), Encoding.UTF8, "application/json")
			});

			Assert.True(response.IsSuccessStatusCode);
			Assert.Equal(HttpStatusCode.NoContent, response.StatusCode);
		}



		private string GetSha256Signature(string json) {
			var config = appFactory.Services.GetRequiredService<IConfiguration>();

			var secret = config["Webhook:Receiver:Signature:Secret"];

			var sha256Signer = new Sha256WebhookSigner();
			return sha256Signer.SignWebhook(json, secret);
		}

		[Fact]
		public async Task ReceiveSignedTestWebhook() {
			var client = CreateClient();

			var json = JsonConvert.SerializeObject(new TestSignedWebhook {
				Id = Guid.NewGuid().ToString("N"),
				Event = "test",
				TimeStamp = DateTimeOffset.Now,
			});

			var sha256Sig = GetSha256Signature(json);

			var request = new HttpRequestMessage(HttpMethod.Post, "/webhook/signed") {
				Content = new StringContent(json, Encoding.UTF8, "application/json")
			};

			request.Headers.TryAddWithoutValidation("X-Webhook-Signature-256", sha256Sig);

			var response = await client.SendAsync(request);

			Assert.True(response.IsSuccessStatusCode);
			Assert.Equal(HttpStatusCode.NoContent, response.StatusCode);
		}

		[Fact]
		public async Task ReceiveSignedTestWebhook_InvalidSignature() {
			var client = CreateClient();

			var json = JsonConvert.SerializeObject(new TestSignedWebhook {
				Id = Guid.NewGuid().ToString("N"),
				Event = "test",
				TimeStamp = DateTimeOffset.Now,
			});

			var sha256Sig = GetSha256Signature(json + "...");

			var request = new HttpRequestMessage(HttpMethod.Post, "/webhook/signed") {
				Content = new StringContent(json, Encoding.UTF8, "application/json")
			};

			request.Headers.TryAddWithoutValidation("X-Webhook-Signature-256", sha256Sig);

			var response = await client.SendAsync(request);

			Assert.False(response.IsSuccessStatusCode);
			Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
		}

		[Fact]
		public async Task ReceiveSignedTestWebhook_NoSignature() {
			var client = CreateClient();

			var json = JsonConvert.SerializeObject(new TestSignedWebhook {
				Id = Guid.NewGuid().ToString("N"),
				Event = "test",
				TimeStamp = DateTimeOffset.Now,
			});

			var request = new HttpRequestMessage(HttpMethod.Post, "/webhook/signed") {
				Content = new StringContent(json, Encoding.UTF8, "application/json")
			};

			var response = await client.SendAsync(request);

			Assert.False(response.IsSuccessStatusCode);
			Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
		}

		[Fact]
		public async Task VerifyReceiver() {
			var client = CreateClient();

			var token = appFactory.Services.GetRequiredService<IConfiguration>()["Webhook:Receiver:VerificationToken"];

			var response = await client.GetAsync($"/webhook/signed?token={token}");

			Assert.True(response.IsSuccessStatusCode);
			Assert.Equal(HttpStatusCode.NoContent, response.StatusCode);
		}

		[Fact]
		public async Task VerifyReceiver_InvalidToken() {
			var client = CreateClient();

			var response = await client.GetAsync($"/webhook/signed?token={Guid.NewGuid().ToString("N")}");

			Assert.False (response.IsSuccessStatusCode);
			Assert.Equal(HttpStatusCode.Forbidden, response.StatusCode);
		}
	}
}
