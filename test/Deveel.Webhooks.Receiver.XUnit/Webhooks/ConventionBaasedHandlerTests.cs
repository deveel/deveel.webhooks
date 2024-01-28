// Copyright 2022-2024 Antonello Provenzano
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

using System.Text;

using Deveel.Webhooks.Model;

using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

using Newtonsoft.Json;

using Xunit;
using Xunit.Abstractions;

namespace Deveel.Webhooks {
	public sealed class ConventionBaasedHandlerTests : ReceiverTestBase<TestWebhook> {
		public ConventionBaasedHandlerTests(ITestOutputHelper outputHelper) : base(outputHelper) {
		}

		private TestWebhook? LastWebhook { get; set; }

		protected override void AddReceiver(IServiceCollection services) {
			services.AddWebhookReceiver<TestWebhook>(options => {
				options.VerifySignature = false;
				options.JsonParser = new NewtonsoftWebhookJsonParser<TestWebhook>();
			});
		}

		private TestServer CreateTestServer(Action<IApplicationBuilder> configure) {
			var webHost = new WebHostBuilder()
				.ConfigureServices(ConfigureServices)
				.Configure(configure);

			return new TestServer(webHost);
		}

		[Fact]
		public async Task SyncHandler() {
			using var server = CreateTestServer(app => {
				app.MapWebhook("/webhook", (TestWebhook webhook) => {
					LastWebhook = webhook;
				});
			});

			var client = server.CreateClient();

			var json = JsonConvert.SerializeObject(new TestWebhook {
				Id = Guid.NewGuid().ToString("N"),
				Event = "test",
				TimeStamp = DateTimeOffset.Now,
			});

			var response = await client.PostAsync("/webhook", new StringContent(json, Encoding.UTF8, "application/json"));

			Assert.True(response.IsSuccessStatusCode);

			Assert.NotNull(LastWebhook);
			Assert.Equal("test", LastWebhook.Event);
			Assert.NotNull(LastWebhook.Id);
		}

		[Fact]
		public async Task SyncHandlerT1() {
			using var server = CreateTestServer(app => {
				app.MapWebhook("/webhook", (TestWebhook webhook, ILogger<TestWebhook> logger) => {
					logger.LogInformation("Received webhook: {id}", webhook.Id);

					LastWebhook = webhook;
				});
			});

			var client = server.CreateClient();

			var json = JsonConvert.SerializeObject(new TestWebhook {
				Id = Guid.NewGuid().ToString("N"),
				Event = "test",
				TimeStamp = DateTimeOffset.Now,
			});

			var response = await client.PostAsync("/webhook", new StringContent(json, Encoding.UTF8, "application/json"));

			Assert.True(response.IsSuccessStatusCode);

			Assert.NotNull(LastWebhook);
			Assert.Equal("test", LastWebhook.Event);
			Assert.NotNull(LastWebhook.Id);
		}

		[Fact]
		public async Task AsyncHandler() {
			using var server = CreateTestServer(app => {
				app.MapWebhook("/webhook", async (TestWebhook webhook) => {
					LastWebhook = webhook;

					await Task.CompletedTask;
				});
			});

			var client = server.CreateClient();

			var json = JsonConvert.SerializeObject(new TestWebhook {
				Id = Guid.NewGuid().ToString("N"),
				Event = "test",
				TimeStamp = DateTimeOffset.Now,
			});

			var response = await client.PostAsync("/webhook", new StringContent(json, Encoding.UTF8, "application/json"));

			Assert.True(response.IsSuccessStatusCode);

			Assert.NotNull(LastWebhook);
			Assert.Equal("test", LastWebhook.Event);
			Assert.NotNull(LastWebhook.Id);
		}
	}
}
