using Bogus;

using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

using Xunit.Abstractions;

namespace Deveel.Webhooks {
	public abstract class DeliveryResultLoggerTestSuite : IAsyncLifetime {
		protected DeliveryResultLoggerTestSuite(ITestOutputHelper testOutput) {
			TestOutput = testOutput;
			SubscriptionFaker = new WebhookSubscriptionFaker(TenantId);
		}

		protected string TenantId { get; } = Guid.NewGuid().ToString();

		protected IServiceProvider? Services { get; private set; }

		protected IServiceScope? Scope { get; private set; }

		protected ITestOutputHelper TestOutput { get; }

		protected IWebhookDeliveryResultLogger<Webhook> ResultLogger 
			=> Scope!.ServiceProvider.GetRequiredService<IWebhookDeliveryResultLogger<Webhook>>();

		protected Faker<Webhook> WebhookFaker { get; } = new WebhookFaker();

		protected Faker<WebhookSubscription> SubscriptionFaker { get; }

		private IServiceProvider BuildServices() {
			var services = new ServiceCollection();

			services.AddLogging(x => x.AddXUnit(TestOutput));

			ConfigureService(services);

			return services.BuildServiceProvider();
		}

		async Task IAsyncLifetime.InitializeAsync() {
			Services = BuildServices();
			Scope = Services.CreateScope();

			await InitializeAsync();
		}

		protected virtual Task InitializeAsync() {
			return Task.CompletedTask;
		}

		async Task IAsyncLifetime.DisposeAsync() {
			await DisposeAsync();

			Scope?.Dispose();
			(Services as IDisposable)?.Dispose();
			Scope = null;
			Services = null;
		}

		protected virtual Task DisposeAsync() {
			return Task.CompletedTask;
		}

		protected virtual void ConfigureService(IServiceCollection services) {
		}

		protected abstract Task<IWebhookDeliveryResult?> FindResultByOperationIdAsync(string operationId);

		[Fact]
		public async Task LogSuccessfulDelivery() {
			var webhook = WebhookFaker.Generate();
			var eventInfo = new EventInfo("test", "executed", "1.0.0", new { name = "logTest" });
			var subscription = SubscriptionFaker.Generate();
			var destination = subscription.AsDestination();

			var result = new WebhookDeliveryResult<Webhook>(Guid.NewGuid().ToString(), destination, webhook);

			var attempt = result.StartAttempt();
			attempt.Complete(200, "OK");

			Assert.True(result.Successful);
			Assert.Single(result.Attempts);
			Assert.Equal(200, result.Attempts[0].ResponseCode);
			Assert.Equal("OK", result.Attempts[0].ResponseMessage);
			
			await ResultLogger.LogResultAsync(eventInfo, subscription, result);

			var logged = await FindResultByOperationIdAsync(result.OperationId);

			Assert.NotNull(logged);
			Assert.Equal(result.OperationId, logged.OperationId);
			Assert.NotNull(logged.Webhook);
			Assert.NotNull(logged.DeliveryAttempts);
			Assert.NotEmpty(logged.DeliveryAttempts);
			Assert.Single(logged.DeliveryAttempts);
			Assert.Equal(200, logged.DeliveryAttempts.ElementAt(0).ResponseStatusCode);
			Assert.Equal("OK", logged.DeliveryAttempts.ElementAt(0).ResponseMessage);
		}

		[Fact]
		public async Task LogFailedDelivery() {
			var webhook = WebhookFaker.Generate();
			var eventInfo = new EventInfo("test", "executed", "1.0.0", new { name = "logTest" });
			var subscription = SubscriptionFaker.Generate();
			var destination = subscription.AsDestination();

			var result = new WebhookDeliveryResult<Webhook>(Guid.NewGuid().ToString(), destination, webhook);

			Enumerable.Range(0, 3)
				.Select(x => result.StartAttempt())
				.ToList()
				.ForEach(x => {
					if (x.Number == 3) {
						x.Complete(200, "OK");
					} else {
						x.Complete(500, "Internal Server Error");
					}
				});

			Assert.True(result.Successful);
			Assert.Equal(3, result.Attempts.Count);
			Assert.Equal(500, result.Attempts[0].ResponseCode);
			Assert.Equal("Internal Server Error", result.Attempts[0].ResponseMessage);
			Assert.Equal(500, result.Attempts[1].ResponseCode);
			Assert.Equal("Internal Server Error", result.Attempts[1].ResponseMessage);
			Assert.Equal(200, result.Attempts[2].ResponseCode);
			Assert.Equal("OK", result.Attempts[2].ResponseMessage);

			await ResultLogger.LogResultAsync(eventInfo, subscription, result);

			var logged = await FindResultByOperationIdAsync(result.OperationId);

			Assert.NotNull(logged);
			Assert.Equal(result.OperationId, logged.OperationId);
			Assert.NotNull(logged.Webhook);
			Assert.NotNull(logged.DeliveryAttempts);
			Assert.NotEmpty(logged.DeliveryAttempts);
			Assert.Equal(3, logged.DeliveryAttempts.Count());
			Assert.Equal(500, logged.DeliveryAttempts.ElementAt(0).ResponseStatusCode);
			Assert.Equal("Internal Server Error", logged.DeliveryAttempts.ElementAt(0).ResponseMessage);
			Assert.Equal(500, logged.DeliveryAttempts.ElementAt(1).ResponseStatusCode);
			Assert.Equal("Internal Server Error", logged.DeliveryAttempts.ElementAt(1).ResponseMessage);
			Assert.Equal(200, logged.DeliveryAttempts.ElementAt(2).ResponseStatusCode);
			Assert.Equal("OK", logged.DeliveryAttempts.ElementAt(2).ResponseMessage);
		}
	}
}
