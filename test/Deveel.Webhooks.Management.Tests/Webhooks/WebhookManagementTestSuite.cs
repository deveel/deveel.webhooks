using Deveel.Data;

using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

using Xunit;
using Xunit.Abstractions;

namespace Deveel.Webhooks {
	public abstract class WebhookManagementTestSuite<TSubscription> : IAsyncLifetime where TSubscription : class, IWebhookSubscription {
		protected WebhookManagementTestSuite(ITestOutputHelper testOutput) {
			TestOutput = testOutput;
		}

		protected ITestOutputHelper TestOutput { get; }

		protected IServiceProvider Services { get; private set; }

		protected IServiceScope Scope { get; private set; }

		protected IReadOnlyList<TSubscription> Subscriptions { get; private set; }

		protected abstract IReadOnlyList<TSubscription> GenerateSubscriptions(int count);

		protected TSubscription GenerateSubscription() => GenerateSubscriptions(1)[0];

		protected WebhookSubscriptionManager<TSubscription> Manager 
			=> Scope.ServiceProvider.GetRequiredService<WebhookSubscriptionManager<TSubscription>>();

		protected IWebhookSubscriptionRepository<TSubscription> Repository 
			=> Scope.ServiceProvider.GetRequiredService<IWebhookSubscriptionRepository<TSubscription>>();

		protected virtual object GenerateSubscriptionKey() => Guid.NewGuid();

		private IServiceProvider BuildServices() {
			var services = new ServiceCollection();

			services.AddLogging(logging => logging.AddXUnit(TestOutput));

			ConfigureServices(services);

			return services.BuildServiceProvider();
		}

		protected virtual void ConfigureWebhooks(IServiceCollection services) {
			services.AddWebhookSubscriptions<TSubscription>(options => {
				ConfigureWebhookStorage(options);
			});
		}

		protected abstract void ConfigureWebhookStorage(WebhookSubscriptionBuilder<TSubscription> options);

		protected virtual void ConfigureServices(IServiceCollection services) {
			ConfigureWebhooks(services);
		}

		protected virtual async Task SeedAsync(IRepository<TSubscription> repository) {
			await repository.AddRangeAsync(Subscriptions);
		}

		async Task IAsyncLifetime.InitializeAsync() {
			Subscriptions = GenerateSubscriptions(120);

			Services = BuildServices();
			Scope = Services.CreateScope();

			await InitializeAsync();
		}

		protected virtual async Task InitializeAsync() {
			await SeedAsync(Repository);
		}

		protected virtual Task DisposeAsync() {
			return Task.CompletedTask;
		}

		async Task IAsyncLifetime.DisposeAsync() {
			await DisposeAsync();

			Scope.Dispose();
			(Services as IDisposable)?.Dispose();
		}

		[Fact]
		public async Task AddSubscription() {
			var subscription = GenerateSubscription();

			await Manager.AddAsync(subscription);

			Assert.NotNull(subscription.SubscriptionId);

			var found = await Repository.FindByKeyAsync(subscription.SubscriptionId);

			Assert.NotNull(found);
			Assert.Equal(subscription.SubscriptionId, found.SubscriptionId);
			Assert.Equal(subscription.Name, found.Name);
			Assert.Equal(subscription.DestinationUrl, found.DestinationUrl);
			Assert.Equal(subscription.Format, found.Format);
			Assert.Equal(subscription.Status, found.Status);
			Assert.Equal(subscription.EventTypes, found.EventTypes);
			Assert.Equal(subscription.Filters, found.Filters);
		}

		[Theory]
		[InlineData("")]
		[InlineData(null)]
		[InlineData("ftp://dest.example.com")]
		[InlineData("test data")]
		public async Task AddSubscriptionWithInvalidDestination(string url) {
			var subscription = GenerateSubscription();
			await Repository.SetDestinationUrlAsync(subscription, url);

			var result = await Manager.AddAsync(subscription);

			Assert.False(result.IsSuccess());
			Assert.True(result.IsError());
			Assert.True(result.IsValidationError());

			var error = Assert.IsType<EntityValidationError>(result.Error);

			Assert.NotNull(error);
			Assert.NotNull(error.ValidationResults);
			Assert.NotEmpty(error.ValidationResults);
			Assert.Single(error.ValidationResults);
		}

		[Fact]
		public async Task FindExistingSubscription() {
			var subscription = Subscriptions.Random();

			var key = Repository.GetEntityKey(subscription);
			Assert.NotNull(key);

			var found = await Manager.FindByKeyAsync(key);

			Assert.NotNull(found);
			Assert.Equal(subscription.SubscriptionId, found.SubscriptionId);
			Assert.Equal(subscription.Name, found.Name);
			Assert.Equal(subscription.DestinationUrl, found.DestinationUrl);
			Assert.Equal(subscription.Format, found.Format);
			Assert.Equal(subscription.Status, found.Status);
			Assert.Equal(subscription.EventTypes, found.EventTypes);
			Assert.Equal(subscription.Filters, found.Filters);
		}

		[Fact]
		public async Task FindNotExistingSubscription() {
			var key = GenerateSubscriptionKey();

			var found = await Manager.FindByKeyAsync(key);

			Assert.Null(found);
		}

		[Fact]
		public async Task RemoveExistingSubscription() {
			var subscription = Subscriptions.Random();
			var key = Repository.GetEntityKey(subscription);
			Assert.NotNull(key);

			var toRemove = await Repository.FindByKeyAsync(key);

			Assert.NotNull(toRemove);

			var result = await Manager.RemoveAsync(toRemove);

			Assert.True(result.IsSuccess());

			var found = await Repository.FindByKeyAsync(key);

			Assert.Null(found);
		}

		[Fact]
		public async Task GetSubscriptionsByEventType() {
			var eventType = Subscriptions.Random(x => x.EventTypes.Any()).EventTypes.ElementAt(0);

			var subscriptions = await Manager.GetByEventTypeAsync(eventType, true);

			Assert.NotNull(subscriptions);
			Assert.NotEmpty(subscriptions);
			Assert.All(subscriptions, x => Assert.Contains(eventType, x.EventTypes));
		}

		[Fact]
		public async Task ActivateSubscription() {
			var subscription = Subscriptions.Random(x => x.Status == WebhookSubscriptionStatus.None);

			var key = Repository.GetEntityKey(subscription);
			Assert.NotNull(key);

			var result = await Manager.SetStatusAsync(subscription, WebhookSubscriptionStatus.Active);

			Assert.True(result.IsSuccess());

			var found = await Repository.FindByKeyAsync(key);

			Assert.NotNull(found);
			Assert.Equal(WebhookSubscriptionStatus.Active, found.Status);
		}
	}
}