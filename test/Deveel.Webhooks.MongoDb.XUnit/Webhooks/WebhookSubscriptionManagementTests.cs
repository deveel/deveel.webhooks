using Bogus;

using Microsoft.Extensions.DependencyInjection;

using MongoDB.Bson;

using MongoFramework.Linq;

using Xunit.Abstractions;

namespace Deveel.Webhooks {
	public class WebhookSubscriptionManagementTests : WebhookServiceTestBase {
		private IList<MongoWebhookSubscription> subscriptions;
		private Faker<MongoWebhookSubscription> faker;

		public WebhookSubscriptionManagementTests(MongoTestCluster mongo, ITestOutputHelper outputHelper) 
			: base(mongo, outputHelper) {
			faker = new Faker<MongoWebhookSubscription>()
				.RuleFor(x => x.Name, f => f.Name.JobTitle())
				.RuleFor(x => x.EventTypes, f => f.Random.ListItems(new[] { "data.created", "data.deleted", "data.updated" }, 2))
				.RuleFor(x => x.Format, f => f.Random.ListItem(new[] { "json", "xml" }))
				.RuleFor(x => x.DestinationUrl, f => f.Internet.UrlWithPath("https"))
				.RuleFor(x => x.Status, f => f.Random.Enum<WebhookSubscriptionStatus>())
				.RuleFor(x => x.Filters, f => new List<MongoWebhookFilter> {
					new MongoWebhookFilter{ Format = "linq", Expression = WebhookFilter.Wildcard }
				});

			subscriptions = new List<MongoWebhookSubscription>();
		}

		private WebhookSubscriptionManager<MongoWebhookSubscription> Manager
			=> Services.GetRequiredService<WebhookSubscriptionManager<MongoWebhookSubscription>>();

		protected IWebhookSubscriptionStore<MongoWebhookSubscription> Store
			=> Services.GetRequiredService<IWebhookSubscriptionStore<MongoWebhookSubscription>>();

		private async Task<string> CreateSubscription(MongoWebhookSubscription subscription) {
			return await Store.CreateAsync(subscription, default);
		}

		private async Task<IWebhookSubscription?> GetSubscription(string subscriptionId)
			=> await Store.FindByIdAsync(subscriptionId, default);

		public override async Task InitializeAsync() {
			await base.InitializeAsync();

			var fakes = faker.Generate(112).ToList();

			foreach (var subscription in fakes) {
				await Store.CreateAsync(subscription, default);

				subscriptions.Add(subscription);
			}
		}

		private MongoWebhookSubscription RandomSubscription()
			=> subscriptions[Random.Shared.Next(0, subscriptions.Count - 1)];

		[Fact]
		public async Task AddSubscription() {
			var subscription = faker.Generate();

			var subscriptionId = await Manager.CreateAsync(subscription);

			Assert.NotNull(subscriptionId);
			Assert.Null(subscription.TenantId);
			Assert.NotEmpty(subscription.EventTypes);
		}

		[Theory]
		[InlineData("")]
		[InlineData(null)]
		[InlineData("ftp://dest.example.com")]
		[InlineData("test data")]
		public async Task AddSubscriptionWithInvalidDestination(string url) {
			var subscription = faker.Generate();
			subscription.DestinationUrl = url;

			var error = await Assert.ThrowsAsync<WebhookSubscriptionValidationException>(
				() => Manager.CreateAsync(subscription));

			Assert.NotNull(error);
			Assert.NotNull(error.Errors);
			Assert.NotEmpty(error.Errors);
			Assert.Single(error.Errors);
		}

		[Fact]
		public async Task RemoveExistingSubscription() {
			var subscription = RandomSubscription();

			var result = await Manager.DeleteAsync(subscription);

			Assert.True(result);
		}

		[Fact]
		public async Task GetExistingSubscription() {
			var subscriptionId = RandomSubscription().Id.ToString();

			var subscription = await Manager.FindByIdAsync(subscriptionId);

			Assert.NotNull(subscription);
			Assert.Equal(subscriptionId, subscription.Id.ToString());
			Assert.NotEmpty(subscription.EventTypes);
			Assert.NotNull(subscription.Filters);
			Assert.NotEmpty(subscription.Filters);
			Assert.Single(subscription.Filters);
			Assert.True(subscription.Filters.First().IsWildcard());
		}

		[Fact]
		public async Task GetNotExistingSubscription() {
			var subscriptionId = ObjectId.GenerateNewId().ToString();

			var subscription = await Manager.FindByIdAsync(subscriptionId);

			Assert.Null(subscription);
		}

		[Fact]
		public async Task GetPageOfSubscriptions() {
			var totalPages = (int) Math.Ceiling(subscriptions.Count / (double) 10);

			var query = new PagedQuery<MongoWebhookSubscription>(1, 10);
			var result = await Manager.GetPageAsync(query);

			Assert.NotNull(result);
			Assert.NotEmpty(result.Items);
			Assert.Equal(subscriptions.Count, result.TotalCount);
			Assert.Equal(totalPages, result.TotalPages);
			Assert.Equal(subscriptions[0].Id.ToString(), result.Items[0].Id.ToString());
			Assert.Equal(subscriptions[1].Id.ToString(), result.Items[1].Id.ToString());
		}

		[Fact]
		public async Task QuerySubscriptions() {
			var dataCreatedSubs = subscriptions.Where(x => x.EventTypes.Any(y => y == "data.created")).ToList();
			var result = await Manager.Subscriptions.Where(x => x.EventTypes.Any(y => y == "data.created")).ToListAsync();

			Assert.Equal(dataCreatedSubs.Count, result.Count);
		}

		[Fact]
		public async Task ActivateExistingSubscription() {
			var subscription = subscriptions.First(x => x.Status == WebhookSubscriptionStatus.Suspended);

			var result = await Manager.EnableAsync(subscription);

			Assert.True(result);

			Assert.Equal(WebhookSubscriptionStatus.Active, subscription.Status);
		}

		[Fact]
		public async Task ActivateAlreadyActiveSubscription() {
			var subscription = subscriptions.First(x => x.Status == WebhookSubscriptionStatus.Active);

			var result = await Manager.EnableAsync(subscription);

			Assert.False(result);
			Assert.Equal(WebhookSubscriptionStatus.Active, subscription.Status);
		}

		[Fact]
		public async Task DisableExistingSubscription() {
			var subscription = subscriptions.First(x => x.Status == WebhookSubscriptionStatus.Active);

			var result = await Manager.DisableAsync(subscription);

			Assert.True(result);
			Assert.Equal(WebhookSubscriptionStatus.Suspended, subscription.Status);

			Assert.True(await Manager.UpdateAsync(subscription));
		}

		[Fact]
		public async Task DisableAlreadyDisabledSubscription() {
			var subscription = subscriptions.First(x => x.Status == WebhookSubscriptionStatus.Suspended);

			var result = await Manager.DisableAsync(subscription);

			Assert.False(result);
			Assert.Equal(WebhookSubscriptionStatus.Suspended, subscription.Status);
		}

		[Fact]
		public async Task CountAllSubscriptionExistingTenant() {
			var count = subscriptions.Count;

			var result = await Manager.CountAllAsync();

			Assert.Equal(count, result);
		}

	}
}