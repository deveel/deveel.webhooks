// Copyright 2022-2025 Antonello Provenzano
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

using System.Text.Json;

using Deveel.Webhooks.Model;

using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

using Xunit;
using Xunit.Abstractions;

namespace Deveel.Webhooks {
	public class FactoryBasedHandlerTests : ReceiverTestBase<TestWebhook> {
		public FactoryBasedHandlerTests(ITestOutputHelper outputHelper) : base(outputHelper) {
		}

		protected override void ConfigureServices(IServiceCollection services) {
			services.AddSingleton(this);
			base.ConfigureServices(services);
		}

		private TestWebhook? LastWebhook { get; set; }

		protected override void AddReceiver(IServiceCollection services) {
			services.AddWebhookReceiver<TestWebhook>(options => {
					options.VerifySignature = false;
				})
				.Handle(webhook => LastWebhook = webhook)
				.Handle((TestWebhook webhook, ILogger<TestWebhook> logger) => {
					LastWebhook = webhook;
					logger.LogInformation("Delegate handler invoked");
					logger.LogInformation(JsonSerializer.Serialize(webhook));
				})
				.AddHandler<TestHandler>();
		}

		private WebhookReceiverMiddleware<TestWebhook> GetMiddleware() {
			var logger = Services.GetRequiredService<ILogger<WebhookReceiverMiddleware<TestWebhook>>>();
			return new WebhookReceiverMiddleware<TestWebhook>(_ => Task.CompletedTask, new WebhookHandlingOptions(), logger);
		}

		[Fact]
		public async Task ReceiveAndHandle() {
			var request = CreateRequestWithJson(JsonSerializer.Serialize(new TestWebhook {
				Id = Guid.NewGuid().ToString(),
				Event = "test",
				TimeStamp = DateTimeOffset.Now,
			}));

			var middleware = GetMiddleware();
			await middleware.InvokeAsync(request.HttpContext);

			Assert.NotNull(LastWebhook);
		}


		class TestHandler : IWebhookHandler<TestWebhook> {
			private readonly FactoryBasedHandlerTests tests;
			private readonly ILogger<TestHandler> logger;

			public TestHandler(FactoryBasedHandlerTests tests, ILogger<TestHandler> logger) {
				this.tests = tests;
				this.logger = logger;
			}

			public Task HandleAsync(TestWebhook webhook, CancellationToken cancellationToken) {
				logger.LogInformation("Service handler invoked");
				logger.LogInformation(JsonSerializer.Serialize(webhook));

				tests.LastWebhook = webhook;

				return Task.CompletedTask;
			}
		}
	}
}
