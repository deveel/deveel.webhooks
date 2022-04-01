using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

using Deveel.Data;
using Deveel.Util;
using Deveel.Webhooks;

using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging.Abstractions;

using Mongo2Go;

using MongoDB.Bson;

using Xunit;

namespace Deveel.Webhooks {
	[Trait("Category", "Webhooks")]
	[Trait("Category", "Subscriptions")]
	public class MultiTenantWebhookManagementTests : IDisposable {
		private readonly MongoDbRunner mongoDbCluster;
		
		private readonly string tenantId = Guid.NewGuid().ToString("N");
		private readonly string userId = Guid.NewGuid().ToString("N");

		private readonly IWebhookSubscriptionManagerProvider<MongoDbWebhookSubscription> webhookManagerProvider;
		private readonly IWebhookSubscriptionStoreProvider<MongoDbWebhookSubscription> storeProvider;

		private readonly IWebhookSubscriptionFactory<MongoDbWebhookSubscription> subscriptionFactory;

		public MultiTenantWebhookManagementTests() {
			mongoDbCluster = MongoDbRunner.Start(logger: NullLogger.Instance);

			var services = new ServiceCollection();
			services.AddWebhooks<MongoDbWebhookSubscription>(buidler => {
				buidler.ConfigureDelivery(options =>
					options.SignWebhooks())
				.UseSubscriptionManager()
				.UseMongoDb(options => {
					options.DatabaseName = "webhooks";
					options.ConnectionString = mongoDbCluster.ConnectionString;
					options.SubscriptionsCollectionName("webhooks_subscription");
				});
			})
				.AddDynamicLinqFilterEvaluator()
				.AddTestHttpClient();

			var provider = services.BuildServiceProvider();

			webhookManagerProvider = provider.GetService<IWebhookSubscriptionManagerProvider<MongoDbWebhookSubscription>>();
			storeProvider = provider.GetRequiredService<IWebhookSubscriptionStoreProvider<MongoDbWebhookSubscription>>();
			subscriptionFactory = provider.GetRequiredService<IWebhookSubscriptionFactory<MongoDbWebhookSubscription>>();
		}

		private async Task<string> CreateSubscription(WebhookSubscriptionInfo subscriptionInfo) {
			var subscription = subscriptionFactory.Create(subscriptionInfo);
			return await storeProvider.CreateAsync(tenantId, subscription);
		}

		private async Task<IWebhookSubscription> GetSubscription(string subscriptionId)
			=> await storeProvider.FindByIdAsync(tenantId, subscriptionId);

		[Fact]
		public async Task AddSubscription() {
			var webhookManager = webhookManagerProvider.GetManager(tenantId);
			var subscriptionId = await webhookManager.AddSubscriptionAsync(userId, new WebhookSubscriptionInfo("test.event", "https://callback.test.io/webhook"), CancellationToken.None);

			Assert.NotNull(subscriptionId);
			var subscription = await storeProvider.FindByIdAsync(tenantId, subscriptionId);

			Assert.NotNull(subscription);
			Assert.Contains("test.event", subscription.EventTypes);
		}

		[Fact]
		public async Task RemoveExistingSubscription() {
			var subscriptionId = await CreateSubscription(new WebhookSubscriptionInfo("test.event", "https://callback.test.io/webhook") {
				Name = "Test Callback"
			});

			var webhookManager = webhookManagerProvider.GetManager(tenantId);
			var result = await webhookManager.RemoveSubscriptionAsync(userId, subscriptionId, default);

			Assert.True(result);

			var subscription = await GetSubscription(subscriptionId);
			Assert.Null(subscription);
		}

		[Fact]
		public async Task RemoveNotExistingSubscription() {
			var subscriptionId = ObjectId.GenerateNewId().ToString();

			var webhookManager = webhookManagerProvider.GetManager(tenantId);
			await Assert.ThrowsAsync<SubscriptionNotFoundException>(() => webhookManager.RemoveSubscriptionAsync(userId, subscriptionId, default));
		}


		[Fact]
		public async Task GetExistingSubscription() {
			var subscriptionId = await CreateSubscription(new WebhookSubscriptionInfo("test.event", "https://callback.test.io/webhook") {
				Filter = WebhookFilter.WildcardFilter ,
				Name = "Test Callback"
			});

			var webhookManager = webhookManagerProvider.GetManager(tenantId);
			var subscription = await webhookManager.GetSubscriptionAsync(subscriptionId, CancellationToken.None);

			Assert.NotNull(subscription);
			Assert.Equal(subscriptionId, subscription.Id.ToString());
			Assert.Contains("test.event", subscription.EventTypes);
			Assert.NotNull(subscription.Filters);
			Assert.NotEmpty(subscription.Filters);
			Assert.Single(subscription.Filters);
			Assert.True(subscription.Filters.First().IsWildcard());
		}

		[Fact]
		public async Task GetNotExistingSubscription() {
			var subscriptionId = ObjectId.GenerateNewId().ToString();

			var webhookManager = webhookManagerProvider.GetManager(tenantId);
			var subscription = await webhookManager.GetSubscriptionAsync( subscriptionId, CancellationToken.None);

			Assert.Null(subscription);
		}

		[Fact]
		public async Task GetPageOfSubscriptions() {
			var subscriptionId1 = await CreateSubscription(new WebhookSubscriptionInfo("test.event", "https://callback.test.io/webhook") {
				Filter =  WebhookFilter.WildcardFilter,
				Name = "Test Callback"
			});

			var subscriptionId2 = await CreateSubscription(new WebhookSubscriptionInfo("test.otherEvent", "https://callback.test.io/webhook") {
				Filter = WebhookFilter.WildcardFilter,
				Name = "Second Test Callback"
			});

			var query = new PagedQuery<MongoDbWebhookSubscription>(1, 10);
			var webhookManager = webhookManagerProvider.GetManager(tenantId);
			var result = await webhookManager.GetSubscriptionsAsync(query, default);

			Assert.NotNull(result);
			Assert.NotEmpty(result.Items);
			Assert.Equal(2, result.TotalCount);
			Assert.Equal(1, result.TotalPages);
			Assert.Equal(subscriptionId1, result.Items.ElementAt(0).Id.ToString());
			Assert.Equal(subscriptionId2, result.Items.ElementAt(1).Id.ToString());
		}

		[Fact]
		public async Task ActivateExistingSubscription() {
			var subscriptionId = await CreateSubscription(new WebhookSubscriptionInfo("test.event", "https://callback.test.io/webhook") {
				Name = "Test Callback",
				Active = false
			});

			var webhookManager = webhookManagerProvider.GetManager(tenantId);
			var result = await webhookManager.EnableSubscriptionAsync(userId, subscriptionId, default);

			Assert.True(result);

			var subscription = await GetSubscription(subscriptionId);

			Assert.NotNull(subscription);
			Assert.Equal(WebhookSubscriptionStatus.Active, subscription.Status);
		}

		[Fact]
		public async Task ActivateNotExistingSubscription() {
			var subscriptionId = ObjectId.GenerateNewId().ToString();

			var webhookManager = webhookManagerProvider.GetManager(tenantId);
			await Assert.ThrowsAsync<SubscriptionNotFoundException>(() => webhookManager.EnableSubscriptionAsync(userId, subscriptionId, default));
		}

		[Fact]
		public async Task ActivateAlreadyActiveSubscription() {
			var subscriptionId = await CreateSubscription(new WebhookSubscriptionInfo("test.event", "https://callback.test.io/webhook") {
				Name = "Test Callback",
				Active = true
			});

			var webhookManager = webhookManagerProvider.GetManager(tenantId);
			var result = await webhookManager.EnableSubscriptionAsync(userId, subscriptionId, default);

			Assert.False(result);

			var subscription = await GetSubscription(subscriptionId);

			Assert.NotNull(subscription);
			Assert.Equal(WebhookSubscriptionStatus.Active, subscription.Status);
		}

		[Fact]
		public async Task DisableExistingSubscription() {
			var subscriptionId = await CreateSubscription(new WebhookSubscriptionInfo("test.event", "https://callback.test.io/webhook") {
				Name = "Test Callback",
				Active = true
			});

			var webhookManager = webhookManagerProvider.GetManager(tenantId);
			var result = await webhookManager.DisableSubscriptionAsync(userId, subscriptionId, default);

			Assert.True(result);

			var subscription = await GetSubscription(subscriptionId);

			Assert.NotNull(subscription);
			Assert.Equal(WebhookSubscriptionStatus.Suspended, subscription.Status);
		}

		[Fact]
		public async Task DisableNoyExistingSubscription() {
			var subscriptionId = ObjectId.GenerateNewId().ToString();

			var webhookManager = webhookManagerProvider.GetManager(tenantId);

			await Assert.ThrowsAsync<SubscriptionNotFoundException>(() => webhookManager.DisableSubscriptionAsync(userId, subscriptionId, default));
		}

		[Fact]
		public async Task DisableAlreadyDisabledSubscription() {
			var subscriptionId = await CreateSubscription(new WebhookSubscriptionInfo("test.event", "https://callback.test.io/webhook") {
				Name = "Test Callback",
				Active = false
			});

			var webhookManager = webhookManagerProvider.GetManager(tenantId);
			var result = await webhookManager.DisableSubscriptionAsync(userId, subscriptionId, default);

			Assert.False(result);

			var subscription = await GetSubscription(subscriptionId);

			Assert.NotNull(subscription);
			Assert.Equal(WebhookSubscriptionStatus.Suspended, subscription.Status);
		}

		[Fact]
		public async Task CountAllSubscriptionExistingTenant() {
			var subscriptionId = await CreateSubscription(new WebhookSubscriptionInfo("test.event", "https://callback.test.io/webhook") {
				Name = "Test Callback",
				Active = false
			});

			var webhookManager = webhookManagerProvider.GetManager(tenantId);
			var result = await webhookManager.CountAllAsync(default);

			Assert.Equal(1, result);
		}

		public void Dispose() {
			mongoDbCluster?.Dispose();
		}
	}
}
