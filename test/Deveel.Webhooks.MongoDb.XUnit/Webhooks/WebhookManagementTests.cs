using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

using Microsoft.Extensions.DependencyInjection;

using MongoDB.Bson;

using Xunit;
using Xunit.Abstractions;

namespace Deveel.Webhooks {
	public class WebhookManagementTests : WebhookServiceTestBase {
		private readonly string userId = Guid.NewGuid().ToString("N");

		private readonly WebhookSubscriptionManager<MongoDbWebhookSubscription> webhookManager;
		private readonly IWebhookSubscriptionStore<MongoDbWebhookSubscription> store;


		public WebhookManagementTests(ITestOutputHelper outputHelper) : base(outputHelper) {
			webhookManager = Services.GetService<WebhookSubscriptionManager<MongoDbWebhookSubscription>>();
			store = Services.GetRequiredService<IWebhookSubscriptionStore<MongoDbWebhookSubscription>>();
		}

		private async Task<string> CreateSubscription(MongoDbWebhookSubscription subscription) {
			return await store.CreateAsync(subscription);
		}

		private async Task<IWebhookSubscription> GetSubscription(string subscriptionId)
			=> await store.FindByIdAsync(subscriptionId);

		[Fact]
		public async Task AddSubscription() {
			var subscriptionId = await webhookManager.AddSubscriptionAsync(new MongoDbWebhookSubscription {
				EventTypes = new List<string> { "test.event" },
				DestinationUrl = "https://callback.test.io/webhook"
			}, default);

			Assert.NotNull(subscriptionId);
			var subscription = await GetSubscription(subscriptionId);

			Assert.NotNull(subscription);
			Assert.Contains("test.event", subscription.EventTypes);
		}

		[Theory]
		[InlineData("")]
		[InlineData(null)]
		[InlineData("ftp://dest.example.com")]
		[InlineData("test data")]
		public async Task AddSubscriptionWithInvalidDestination(string url) {
			var subscription = new MongoDbWebhookSubscription { EventTypes = new List<string> { "test.event" }, DestinationUrl = url };
			var error = await Assert.ThrowsAsync<WebhookSubscriptionValidationException>(() => webhookManager.AddSubscriptionAsync(subscription, default));

			Assert.NotNull(error);
			Assert.NotEmpty(error.Errors);
			Assert.Single(error.Errors);
		}

		[Fact]
		public async Task RemoveExistingSubscription() {
			var subscriptionId = await CreateSubscription(new MongoDbWebhookSubscription { 
				EventTypes = new List<string> { "test.event" }, 
				DestinationUrl = "https://callback.test.io/webhook",
				Name = "Test Callback"
			});

			var result = await webhookManager.RemoveSubscriptionAsync(subscriptionId, default);

			Assert.True(result);

			var subscription = await GetSubscription(subscriptionId);
			Assert.Null(subscription);
		}

		[Fact]
		public async Task RemoveNotExistingSubscription() {
			var subscriptionId = ObjectId.GenerateNewId().ToString();

			await Assert.ThrowsAsync<SubscriptionNotFoundException>(() => webhookManager.RemoveSubscriptionAsync(subscriptionId, default));
		}


		[Fact]
		public async Task GetExistingSubscription() {
			var subscriptionId = await CreateSubscription(new MongoDbWebhookSubscription {
				EventTypes = new List<string> { "test.event" }, 
				DestinationUrl = "https://callback.test.io/webhook",
				Filters = new List<MongoDbWebhookFilter> { 
					new MongoDbWebhookFilter {
						Format = "linq",
						Expression = WebhookFilter.Wildcard
					}
				},
				Name = "Test Callback"
			});

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

			var subscription = await webhookManager.GetSubscriptionAsync(subscriptionId, CancellationToken.None);

			Assert.Null(subscription);
		}

		[Fact]
		public async Task GetPageOfSubscriptions() {
			var subscriptionId1 = await CreateSubscription(new MongoDbWebhookSubscription { 
				EventTypes = new List<string> { "test.event" }, 
				DestinationUrl = "https://callback.test.io/webhook",
				Filters = new List<MongoDbWebhookFilter> {
					new MongoDbWebhookFilter {
						Format = "linq",
						Expression = WebhookFilter.Wildcard
					}
				},
				Name = "Test Callback"
			});

			var subscriptionId2 = await CreateSubscription(new MongoDbWebhookSubscription { 
				EventTypes = new List<string> { "test.otherEvent" }, 
				DestinationUrl = "https://callback.test.io/webhook",
				Filters = new List<MongoDbWebhookFilter> {
					new MongoDbWebhookFilter {
						Format = "linq",
						Expression = WebhookFilter.Wildcard
					}
				},
				Name = "Second Test Callback"
			});

			var query = new PagedQuery<MongoDbWebhookSubscription>(1, 10);
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
			var subscriptionId = await CreateSubscription(new MongoDbWebhookSubscription {
				EventTypes = new List<string> { "test.event" }, 
				DestinationUrl = "https://callback.test.io/webhook",
				Name = "Test Callback",
				Status = WebhookSubscriptionStatus.Suspended
			});

			var result = await webhookManager.EnableSubscriptionAsync(subscriptionId, default);

			Assert.True(result);

			var subscription = await GetSubscription(subscriptionId);

			Assert.NotNull(subscription);
			Assert.Equal(WebhookSubscriptionStatus.Active, subscription.Status);
		}

		[Fact]
		public async Task ActivateNotExistingSubscription() {
			var subscriptionId = ObjectId.GenerateNewId().ToString();

			await Assert.ThrowsAsync<SubscriptionNotFoundException>(() => webhookManager.EnableSubscriptionAsync(subscriptionId, default));
		}

		[Fact]
		public async Task ActivateAlreadyActiveSubscription() {
			var subscriptionId = await CreateSubscription(new MongoDbWebhookSubscription { 
				EventTypes = new List<string> { "test.event" }, 
				DestinationUrl = "https://callback.test.io/webhook",
				Name = "Test Callback",
				Status = WebhookSubscriptionStatus.Active
			});

			var result = await webhookManager.EnableSubscriptionAsync(subscriptionId, default);

			Assert.False(result);

			var subscription = await GetSubscription(subscriptionId);

			Assert.NotNull(subscription);
			Assert.Equal(WebhookSubscriptionStatus.Active, subscription.Status);
		}

		[Fact]
		public async Task DisableExistingSubscription() {
			var subscriptionId = await CreateSubscription(new MongoDbWebhookSubscription { 
				EventTypes = new List<string> { "test.event" }, 
				DestinationUrl = "https://callback.test.io/webhook", 
				Name = "Test Callback", 
				Status = WebhookSubscriptionStatus.Active 
			});

			var result = await webhookManager.DisableSubscriptionAsync(subscriptionId, default);

			Assert.True(result);

			var subscription = await GetSubscription(subscriptionId);

			Assert.NotNull(subscription);
			Assert.Equal(WebhookSubscriptionStatus.Suspended, subscription.Status);
		}

		[Fact]
		public async Task DisableNoyExistingSubscription() {
			var subscriptionId = ObjectId.GenerateNewId().ToString();

			await Assert.ThrowsAsync<SubscriptionNotFoundException>(() => webhookManager.DisableSubscriptionAsync(subscriptionId, default));
		}

		[Fact]
		public async Task DisableAlreadyDisabledSubscription() {
			var subscriptionId = await CreateSubscription(new MongoDbWebhookSubscription { 
				EventTypes = new List<string> { "test.event" }, 
				DestinationUrl = "https://callback.test.io/webhook", 
				Name = "Test Callback", 
				Status = WebhookSubscriptionStatus.Suspended 
			});

			var result = await webhookManager.DisableSubscriptionAsync(subscriptionId, default);

			Assert.False(result);

			var subscription = await GetSubscription(subscriptionId);

			Assert.NotNull(subscription);
			Assert.Equal(WebhookSubscriptionStatus.Suspended, subscription.Status);
		}

		[Fact]
		public async Task CountAllSubscriptionExistingTenant() {
			var subscriptionId = await CreateSubscription(new MongoDbWebhookSubscription { 
				EventTypes = new List<string> { "test.event" }, 
				DestinationUrl = "https://callback.test.io/webhook", 
				Name = "Test Callback", 
				Status = WebhookSubscriptionStatus.Suspended 
			});

			var result = await webhookManager.CountAllAsync(default);

			Assert.Equal(1, result);
		}

	}
}