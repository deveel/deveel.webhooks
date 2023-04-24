using Bogus;

using Finbuckle.MultiTenant;
using Finbuckle.MultiTenant.Stores;

using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

using MongoDB.Driver;

using Xunit.Abstractions;

namespace Deveel.Webhooks {
	public class MultiTenantWebhookSubscriptionManagementTests : WebhookServiceTestBase {
		private IList<MongoWebhookSubscription> subscriptions;
		private IList<MongoWebhookSubscription> otherSubscriptions;
		private Faker<MongoWebhookSubscription> faker;


		public MultiTenantWebhookSubscriptionManagementTests(MongoTestCluster mongo, ITestOutputHelper outputHelper) 
			: base(mongo, outputHelper) {
			faker = new Faker<MongoWebhookSubscription>()
				.RuleFor(x => x.Name, f => f.Name.JobTitle())
				.RuleFor(x => x.EventTypes, f => f.Random.ListItems(new[] { "data.created", "data.deleted" }))
				.RuleFor(x => x.Format, f => f.Random.ListItem(new[] { "json", "xml" }))
				.RuleFor(x => x.DestinationUrl, f => f.Internet.UrlWithPath("https"))
				.RuleFor(x => x.Status, f => f.Random.Enum<WebhookSubscriptionStatus>())
				.RuleFor(x => x.Filters, f => new List<MongoWebhookFilter> {
					new MongoWebhookFilter{ Format = "linq", Expression = WebhookFilter.Wildcard }
				});

			subscriptions = new List<MongoWebhookSubscription>();
			otherSubscriptions = new List<MongoWebhookSubscription>();
		}

		private string TenantId { get; } = Guid.NewGuid().ToString();

		private string OtherTenantId { get; } = Guid.NewGuid().ToString();

		private WebhookSubscriptionManager<MongoWebhookSubscription> Manager
			=> Services.GetRequiredService<WebhookSubscriptionManager<MongoWebhookSubscription>>();

		protected IWebhookSubscriptionStoreProvider<MongoWebhookSubscription> StoreProvider
			=> Services.GetRequiredService<IWebhookSubscriptionStoreProvider<MongoWebhookSubscription>>();

		private async Task<string> CreateSubscription(string tenantId, MongoWebhookSubscription subscription) {
			var store = StoreProvider.GetTenantStore(tenantId);

			return await store.CreateAsync(subscription, default);
		}

		private async Task<IWebhookSubscription?> GetSubscription(string tenantId, string subscriptionId)
			=> await StoreProvider.GetTenantStore(tenantId).FindByIdAsync(subscriptionId, default);


		protected override void ConfigureServices(IServiceCollection services) {
			services.AddSingleton<IMultiTenantContext<TenantInfo>>(_ => new MultiTenantContext<TenantInfo> {
				TenantInfo = new TenantInfo {
					Id = TenantId,
					Identifier = TenantId,
					Name = "Test Tenant",
					ConnectionString = $"{ConnectionString}webhooks_1"
				}
			});

			services.AddSingleton<IMultiTenantStore<TenantInfo>>(_ => {
				var store = new InMemoryStore<TenantInfo>(Options.Create(new InMemoryStoreOptions<TenantInfo> {
					Tenants = new List<TenantInfo> {
						new TenantInfo {
							Id = TenantId,
							Identifier = TenantId,
							Name = "Test Tenant",
							ConnectionString = $"{ConnectionString}webhooks_1"
						},
						new TenantInfo {
							Id = OtherTenantId,
							Identifier = OtherTenantId,
							Name = "Other Tenant",
							ConnectionString = $"{ConnectionString}webhooks_2"
						}
					}
				}));

				return store;
			});

			base.ConfigureServices(services);
		}

		protected override void ConfigureWebhookService(WebhookSubscriptionBuilder<MongoWebhookSubscription> builder) {
			builder.UseMongoDb(mongo => {
				mongo.UseMultiTenant();
			});
		}

		public override async Task InitializeAsync() {
			var client = new MongoClient(ConnectionString);
			await client.GetDatabase("webhooks_1").CreateCollectionAsync(MongoDbWebhookStorageConstants.SubscriptionCollectionName);
			await client.GetDatabase("webhooks_1").CreateCollectionAsync(MongoDbWebhookStorageConstants.DeliveryResultsCollectionName);

			await client.GetDatabase("webhooks_2").CreateCollectionAsync(MongoDbWebhookStorageConstants.SubscriptionCollectionName);
			await client.GetDatabase("webhooks_2").CreateCollectionAsync(MongoDbWebhookStorageConstants.DeliveryResultsCollectionName);


			var fakes = faker.Generate(112).ToList();

			foreach (var subscription in fakes) {
				await CreateSubscription(TenantId, subscription);

				subscriptions.Add(subscription);
			}

			var otherFakes = faker.Generate(12).ToList();

			foreach (var subscription in otherFakes) {
				await CreateSubscription(OtherTenantId, subscription);

				otherSubscriptions.Add(subscription);
			}
		}

		public override async Task DisposeAsync() {
			var client = new MongoClient(ConnectionString);

			await client.GetDatabase("webhooks_1").DropCollectionAsync(MongoDbWebhookStorageConstants.SubscriptionCollectionName);
			await client.GetDatabase("webhooks_1").DropCollectionAsync(MongoDbWebhookStorageConstants.DeliveryResultsCollectionName);

			await client.GetDatabase("webhooks_2").DropCollectionAsync(MongoDbWebhookStorageConstants.SubscriptionCollectionName);
			await client.GetDatabase("webhooks_2").DropCollectionAsync(MongoDbWebhookStorageConstants.DeliveryResultsCollectionName);
		}

		private MongoWebhookSubscription RandomSubscription()
			=> subscriptions[Random.Shared.Next(0, subscriptions.Count - 1)];

		private MongoWebhookSubscription RandomOtherSubscription()
			=> otherSubscriptions[Random.Shared.Next(0, otherSubscriptions.Count - 1)];

		[Fact]
		public async Task AddSubscription() {
			var subscription = new MongoWebhookSubscription {
				EventTypes = new List<string> { "test.event" },
				DestinationUrl = "https://callback.test.io/webhook"
			};
			var subscriptionId = await Manager.CreateAsync(subscription);

			Assert.NotNull(subscriptionId);
			Assert.Equal(subscriptionId, subscription.Id.ToString());
			Assert.NotNull(subscription.TenantId);
			Assert.Equal(TenantId, subscription.TenantId);
			Assert.Contains("test.event", subscription.EventTypes);
		}

		[Theory]
		[InlineData("")]
		[InlineData(null)]
		[InlineData("ftp://dest.example.com")]
		[InlineData("test data")]
		public async Task AddSubscriptionWithInvalidDestination(string url) {
			var subscription = new MongoWebhookSubscription { 
				EventTypes = new List<string> { "test.event" }, 
				DestinationUrl = url 
			};

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

			Assert.Equal(TenantId, subscription.TenantId);

			var result = await Manager.DeleteAsync(subscription);

			Assert.True(result);
		}

		[Fact]
		public async Task RemoveSubscriptionFromOtherTenant() {
			var subscription = RandomOtherSubscription();

			var result = await Manager.DeleteAsync(subscription);

			Assert.False(result);
		}

		[Fact]
		public async Task RemoveSubscriptionWithDifferentTenant() {
			var subscription = RandomSubscription();

			subscription.TenantId = OtherTenantId;

			await Assert.ThrowsAsync<WebhookMongoException>(() => Manager.DeleteAsync(subscription));
		}

	}
}
