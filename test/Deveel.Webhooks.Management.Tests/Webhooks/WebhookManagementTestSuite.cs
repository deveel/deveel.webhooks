using System.Security.Principal;

using Bogus;

using Deveel.Data;

using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

using Xunit;
using Xunit.Abstractions;

namespace Deveel.Webhooks {
	public abstract class WebhookManagementTestSuite<TSubscription, TKey> : IAsyncLifetime 
		where TSubscription : class, IWebhookSubscription
		where TKey : notnull {
		protected WebhookManagementTestSuite(ITestOutputHelper testOutput) {
			TestOutput = testOutput;
		}

		protected ITestOutputHelper TestOutput { get; }

		protected IServiceProvider? Services { get; private set; }

		protected IServiceScope? Scope { get; private set; }

		protected IReadOnlyList<TSubscription>? Subscriptions { get; private set; }

		protected abstract Faker<TSubscription> Faker { get; }

		protected IReadOnlyList<TSubscription> GenerateSubscriptions(int count) => Faker.Generate(count);

		protected TSubscription GenerateSubscription(Func<TSubscription, bool>? condition = null, int maxRetries = 100) {
			var subscription = Faker.Generate();
			var retry = 0;

			while (true) {
				if (condition == null || condition(subscription))
					return subscription;

				subscription = Faker.Generate();
				if (++retry > maxRetries)
					throw new InvalidOperationException("Unable to generate the subscription mock");
			}
		}

		protected WebhookSubscriptionManager<TSubscription, TKey> Manager 
			=> Scope!.ServiceProvider.GetRequiredService<WebhookSubscriptionManager<TSubscription, TKey>>();

		protected IWebhookSubscriptionRepository<TSubscription,TKey> Repository 
			=> Scope!.ServiceProvider.GetRequiredService<IWebhookSubscriptionRepository<TSubscription,TKey>>();

		protected abstract TKey GenerateSubscriptionKey();

		private IServiceProvider BuildServices() {
			var services = new ServiceCollection();

			services.AddLogging(logging => logging.AddXUnit(TestOutput));

			ConfigureServices(services);

			return services.BuildServiceProvider();
		}

		protected virtual void ConfigureWebhooks(IServiceCollection services) {
			services.AddWebhookSubscriptions<TSubscription,TKey>(options => {
				ConfigureWebhookStorage(options);
			});
		}

		protected abstract void ConfigureWebhookStorage(WebhookSubscriptionBuilder<TSubscription, TKey> options);

		protected virtual void ConfigureServices(IServiceCollection services) {
			ConfigureWebhooks(services);
		}

		protected virtual async Task SeedAsync(IRepository<TSubscription, TKey> repository) {
			await repository.AddRangeAsync(Subscriptions!);
		}

		protected virtual async Task ClearAsync(IRepository<TSubscription, TKey> repository) {
			await repository.RemoveRangeAsync(Subscriptions!);
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

		protected async virtual Task DisposeAsync() {
			await ClearAsync(Repository);
		}

		async Task IAsyncLifetime.DisposeAsync() {
			await DisposeAsync();

			Scope?.Dispose();
			Scope = null;
			(Services as IDisposable)?.Dispose();
			Services = null;
		}

		[Fact]
		public async Task AddSubscription() {
			var subscription = GenerateSubscription(x => x.Status == WebhookSubscriptionStatus.None);

			var result = await Manager.AddAsync(subscription);

			Assert.True(result.IsSuccess());

			Assert.NotNull(subscription.SubscriptionId);

			var key = Repository.GetEntityKey(subscription);
			var found = await Repository.FindAsync(key!);

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
#pragma warning disable xUnit1012 // Null should only be used for nullable parameters
		[InlineData(null)]
#pragma warning restore xUnit1012 // Null should only be used for nullable parameters
		[InlineData("ftp://dest.example.com")]
		[InlineData("test data")]
		public async Task AddSubscriptionWithInvalidDestination(string url) {
			var subscription = GenerateSubscription();
			await Repository.SetDestinationUrlAsync(subscription, url);

			var result = await Manager.AddAsync(subscription);

			Assert.False(result.IsSuccess());
			Assert.True(result.IsError());
			Assert.True(result.HasValidationErrors());

			var error = Assert.IsAssignableFrom<IValidationError>(result.Error);

			Assert.NotNull(error);
			Assert.NotNull(error.ValidationResults);
			Assert.NotEmpty(error.ValidationResults);
			Assert.Contains(error.ValidationResults, x => x.MemberNames.Contains(nameof(IWebhookSubscription.DestinationUrl)));
		}

		[Fact]
		public async Task SetNewDestinationUrl() {
			var subscription = Subscriptions!.Random();

			var result = await Manager.SetDestinationUrlAsync(subscription, "http://new.example.com");

			Assert.True(result.IsSuccess());
			Assert.False(result.IsError());

			var key = Repository.GetEntityKey(subscription);
			var updated = await Repository.FindAsync(key!);

			Assert.NotNull(updated);
			Assert.Equal("http://new.example.com", updated.DestinationUrl);
		}

		[Theory]
		[InlineData("")]
		[InlineData(null)]
		[InlineData("ftp://dest.example.com")]
		[InlineData("test data")]
		public async Task SetInvalidDestinationUrl(string url) {
			var subscription = Subscriptions!.Random();

			var result = await Manager.SetDestinationUrlAsync(subscription, url);

			Assert.False(result.IsSuccess());
			Assert.True(result.IsError());
			Assert.True(result.HasValidationErrors());

			var error = Assert.IsAssignableFrom<IValidationError>(result.Error);

			Assert.NotNull(error);
			Assert.NotNull(error.ValidationResults);
			Assert.NotEmpty(error.ValidationResults);
			Assert.Contains(error.ValidationResults, x => x.MemberNames.Contains(nameof(IWebhookSubscription.DestinationUrl)));
		}

		[Fact]
		public async Task GetCurrentStatus() {
			var subscription = Subscriptions!.Random();

			var status = await Manager.GetStatusAsync(subscription);

			Assert.Equal(subscription.Status, status);
		}

		[Fact]
		public async Task SetEventTypes_AddNew() {
			var subscription = Subscriptions!.Random();

			var eventTypes = new List<string>(subscription.EventTypes);
			eventTypes.Add("user.created");

			var result = await Manager.SetEventTypesAsync(subscription, eventTypes.ToArray());

			Assert.True(result.IsSuccess());
			Assert.False(result.IsError());

			var key = Repository.GetEntityKey(subscription);
			var updated = await Repository.FindAsync(key!);

			Assert.NotNull(updated);
			Assert.Contains("user.created", updated.EventTypes);
		}

		[Fact]
		public async Task SetEventTypes_Remove() {
			var subscription = Subscriptions!.Random(x => x.EventTypes.Count() > 1);

			var eventTypeToRemove = subscription.EventTypes.ElementAt(0);
			var newEvents = subscription.EventTypes.Except(new[] {eventTypeToRemove});

			var result = await Manager.SetEventTypesAsync(subscription, newEvents.ToArray());

			Assert.True(result.IsSuccess());
			Assert.False(result.IsError());

			var key = Repository.GetEntityKey(subscription);
			var updated = await Repository.FindAsync(key!);

			Assert.NotNull(updated);
			Assert.DoesNotContain(eventTypeToRemove, updated.EventTypes);
		}

		[Fact]
		public async Task SetEventTypes_SameEvents() {
			var subscription = Subscriptions!.Random();

			var result = await Manager.SetEventTypesAsync(subscription, subscription.EventTypes.ToArray());

			Assert.False(result.IsSuccess());
			Assert.True(result.IsUnchanged());
		}

		[Fact]
		public async Task SetNewSecret() {
			var subscription = Subscriptions!.Random();

			var secret = new Faker().Internet.Password(20);

			var result = await Manager.SetSecretAsync(subscription, secret);

			Assert.True(result.IsSuccess());
			Assert.False(result.IsError());

			var key = Repository.GetEntityKey(subscription);
			var updated = await Repository.FindAsync(key!);

			Assert.NotNull(updated);
			Assert.Equal(secret, updated.Secret);
		}

		[Fact]
		public async Task SetSameSecret() {
			var subscription = Subscriptions!.Random(x => x.Secret != null);

			var result = await Manager.SetSecretAsync(subscription, subscription.Secret);

			Assert.False(result.IsSuccess());
			Assert.True(result.IsUnchanged());
		}

		[Fact]
		public async Task RemoveSecret() {
			var subscription = Subscriptions!.Random(x => x.Secret != null);

			var result = await Manager.SetSecretAsync(subscription, null);

			Assert.True(result.IsSuccess());
			Assert.False(result.IsError());

			var key = Repository.GetEntityKey(subscription);
			var updated = await Repository.FindAsync(key!);

			Assert.NotNull(updated);
			Assert.Null(updated.Secret);
		}

		[Fact]
		public async Task FindExistingSubscription() {
			var subscription = Subscriptions!.Random();

			var key = Repository.GetEntityKey(subscription);
			Assert.NotNull(key);

			var found = await Manager.FindAsync(key);

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

			var found = await Manager.FindAsync(key);

			Assert.Null(found);
		}

		[Fact]
		public async Task RemoveExistingSubscription() {
			var subscription = Subscriptions!.Random();
			var key = Repository.GetEntityKey(subscription);
			Assert.NotNull(key);

			var toRemove = await Repository.FindAsync(key);

			Assert.NotNull(toRemove);

			var result = await Manager.RemoveAsync(toRemove);

			Assert.True(result.IsSuccess());

			var found = await Repository.FindAsync(key);

			Assert.Null(found);
		}

		[Fact]
		public async Task GetSubscriptionsByEventType() {
			var eventType = Subscriptions!.Random(x => x.EventTypes.Any()).EventTypes.ElementAt(0);

			var subscriptions = await Manager.GetByEventTypeAsync(eventType, true);

			Assert.NotNull(subscriptions);
			Assert.NotEmpty(subscriptions);
			Assert.All(subscriptions, x => Assert.Contains(eventType, x.EventTypes));
		}

		[Fact]
		public async Task ActivateSubscription() {
			var subscription = Subscriptions!.Random(x => x.Status == WebhookSubscriptionStatus.None);

			var key = Repository.GetEntityKey(subscription);
			Assert.NotNull(key);

			var result = await Manager.SetStatusAsync(subscription, WebhookSubscriptionStatus.Active);

			Assert.True(result.IsSuccess());

			var updated = await Repository.FindAsync(key);

			Assert.NotNull(updated);
			Assert.Equal(WebhookSubscriptionStatus.Active, updated.Status);
		}

		[Fact]
		public async Task ActivateActiveSubscription() {
			var subscription = Subscriptions!.Random(x => x.Status == WebhookSubscriptionStatus.Active);

			var result = await Manager.SetStatusAsync(subscription, WebhookSubscriptionStatus.Active);

			Assert.False(result.IsSuccess());
			Assert.True(result.IsUnchanged());
		}

		[Fact]
		public async Task SuspendSubscription() {
			var subscription = Subscriptions!.Random(x => x.Status == WebhookSubscriptionStatus.Active);

			var key = Repository.GetEntityKey(subscription);
			Assert.NotNull(key);

			var result = await Manager.SetStatusAsync(subscription, WebhookSubscriptionStatus.Suspended);

			Assert.True(result.IsSuccess());

			var found = await Repository.FindAsync(key);

			Assert.NotNull(found);
			Assert.Equal(WebhookSubscriptionStatus.Suspended, found.Status);
		}

		[Fact]
		public async Task AddNewHeaders() {
			var subscription = Subscriptions!.Random();

			var headers = new Dictionary<string, string> {
				{"X-Test-Header", "test value"}
			};

			var result = await Manager.SetHeadersAsync(subscription, headers);

			Assert.True(result.IsSuccess());

			var key = Repository.GetEntityKey(subscription);
			var updated = await Repository.FindAsync(key!);

			Assert.NotNull(updated);

			Assert.NotNull(updated.Headers);
			Assert.NotEmpty(updated.Headers);
			Assert.Contains(updated.Headers, x => x.Key == "X-Test-Header" && x.Value == "test value");
		}

		[Fact]
		public async Task AddExistingHeaders() {
			var subscription = Subscriptions!.Random(x => x.Headers?.Any() ?? false);

			var headers = subscription.Headers!.ToDictionary(x => x.Key, x => x.Value);

			await Manager.SetHeadersAsync(subscription, headers);

			var result = await Manager.SetHeadersAsync(subscription, headers);

			Assert.False(result.IsSuccess());
			Assert.True(result.IsUnchanged());
		}

		[Fact]
		public async Task AddNewProperties() {
			var subscription = Subscriptions!.Random();

			var properties = new Dictionary<string, object> {
				{"testProperty", "test value"},
				{ "testProperty2", 220 }
			};

			var result = await Manager.SetPropertiesAsync(subscription, properties);

			Assert.True(result.IsSuccess());
			Assert.False(result.IsUnchanged());

			var key = Repository.GetEntityKey(subscription);
			var updated = await Repository.FindAsync(key!);

			Assert.NotNull(updated);
			Assert.NotNull(updated.Properties);
			Assert.NotEmpty(updated.Properties);
			Assert.Contains(updated.Properties, x => x.Key == "testProperty" && (string) x.Value == "test value");
			Assert.Contains(updated.Properties, x => x.Key == "testProperty2" && (int) x.Value == 220);
		}

		[Fact]
		public async Task GetSimplePage() {
			var totalPages = (int)Math.Ceiling(Subscriptions!.Count / (double)10);

			Assert.True(Manager.SupportsPaging);

			var query = new PageQuery<TSubscription>(1, 10);
			var result = await Manager.GetPageAsync(query);

			Assert.NotNull(result);
			Assert.NotNull(result.Items);
			Assert.NotEmpty(result.Items);
			Assert.Equal(Subscriptions.Count, result.TotalItems);
			Assert.Equal(totalPages, result.TotalPages);
		}

		[Fact]
		public async Task GetPageWithFilter() {
			var items = Subscriptions!.Where(x => x.Status == WebhookSubscriptionStatus.Active).ToList();
			var totalPages = (int)Math.Ceiling(items.Count / (double)10);

			Assert.True(Manager.SupportsPaging);

			var query = new PageQuery<TSubscription>(1, 10)
				.Where(x => x.Status == WebhookSubscriptionStatus.Active);

			var result = await Manager.GetPageAsync(query);

			Assert.NotNull(result);
			Assert.NotNull(result.Items);
			Assert.NotEmpty(result.Items);
			Assert.Equal(items.Count, result.TotalItems);
			Assert.Equal(totalPages, result.TotalPages);
		}

		[Fact]
		public async Task CountAllSubscriptions() {
			var subsCount = Subscriptions!.Count;
			var count = await Manager.CountAsync();

			Assert.Equal(subsCount, count);
		}
	}
}