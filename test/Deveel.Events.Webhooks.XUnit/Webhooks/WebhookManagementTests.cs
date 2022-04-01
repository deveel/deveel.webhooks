﻿using System;
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

		private readonly IWebhookSubscriptionManager<MongoDbWebhookSubscription> webhookManager;
		private readonly IWebhookSubscriptionStore<MongoDbWebhookSubscription> store;
		private readonly IWebhookSubscriptionFactory<MongoDbWebhookSubscription> subscriptionFactory;


		public WebhookManagementTests(ITestOutputHelper outputHelper) : base(outputHelper) {
			webhookManager = Services.GetService<IWebhookSubscriptionManager<MongoDbWebhookSubscription>>();
			store = Services.GetRequiredService<IWebhookSubscriptionStore<MongoDbWebhookSubscription>>();
			subscriptionFactory = Services.GetRequiredService<IWebhookSubscriptionFactory<MongoDbWebhookSubscription>>();
		}

		private async Task<string> CreateSubscription(WebhookSubscriptionInfo subscriptionInfo) {
			var subscription = subscriptionFactory.Create(subscriptionInfo);
			return await store.CreateAsync(subscription);
		}

		private async Task<IWebhookSubscription> GetSubscription(string subscriptionId)
			=> await store.FindByIdAsync(subscriptionId);

		[Fact]
		public async Task AddSubscription() {
			var subscriptionId = await webhookManager.AddSubscriptionAsync(userId, new WebhookSubscriptionInfo("test.event", "https://callback.test.io/webhook"), CancellationToken.None);

			Assert.NotNull(subscriptionId);
			var subscription = await GetSubscription(subscriptionId);

			Assert.NotNull(subscription);
			Assert.Contains("test.event", subscription.EventTypes);
		}

		[Fact]
		public async Task RemoveExistingSubscription() {
			var subscriptionId = await CreateSubscription(new WebhookSubscriptionInfo("test.event", "https://callback.test.io/webhook") {
				Name = "Test Callback"
			});

			var result = await webhookManager.RemoveSubscriptionAsync(userId, subscriptionId, default);

			Assert.True(result);

			var subscription = await GetSubscription(subscriptionId);
			Assert.Null(subscription);
		}

		[Fact]
		public async Task RemoveNotExistingSubscription() {
			var subscriptionId = ObjectId.GenerateNewId().ToString();

			await Assert.ThrowsAsync<SubscriptionNotFoundException>(() => webhookManager.RemoveSubscriptionAsync(userId, subscriptionId, default));
		}


		[Fact]
		public async Task GetExistingSubscription() {
			var subscriptionId = await CreateSubscription(new WebhookSubscriptionInfo("test.event", "https://callback.test.io/webhook") {
				Filter = WebhookFilter.WildcardFilter,
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
			var subscriptionId1 = await CreateSubscription(new WebhookSubscriptionInfo("test.event", "https://callback.test.io/webhook") {
				Filter = WebhookFilter.WildcardFilter,
				Name = "Test Callback"
			});

			var subscriptionId2 = await CreateSubscription(new WebhookSubscriptionInfo("test.otherEvent", "https://callback.test.io/webhook") {
				Filter = WebhookFilter.WildcardFilter,
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
			var subscriptionId = await CreateSubscription(new WebhookSubscriptionInfo("test.event", "https://callback.test.io/webhook") {
				Name = "Test Callback",
				Active = false
			});

			var result = await webhookManager.EnableSubscriptionAsync(userId, subscriptionId, default);

			Assert.True(result);

			var subscription = await GetSubscription(subscriptionId);

			Assert.NotNull(subscription);
			Assert.Equal(WebhookSubscriptionStatus.Active, subscription.Status);
		}

		[Fact]
		public async Task ActivateNotExistingSubscription() {
			var subscriptionId = ObjectId.GenerateNewId().ToString();

			await Assert.ThrowsAsync<SubscriptionNotFoundException>(() => webhookManager.EnableSubscriptionAsync(userId, subscriptionId, default));
		}

		[Fact]
		public async Task ActivateAlreadyActiveSubscription() {
			var subscriptionId = await CreateSubscription(new WebhookSubscriptionInfo("test.event", "https://callback.test.io/webhook") {
				Name = "Test Callback",
				Active = true
			});

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

			var result = await webhookManager.DisableSubscriptionAsync(userId, subscriptionId, default);

			Assert.True(result);

			var subscription = await GetSubscription(subscriptionId);

			Assert.NotNull(subscription);
			Assert.Equal(WebhookSubscriptionStatus.Suspended, subscription.Status);
		}

		[Fact]
		public async Task DisableNoyExistingSubscription() {
			var subscriptionId = ObjectId.GenerateNewId().ToString();

			await Assert.ThrowsAsync<SubscriptionNotFoundException>(() => webhookManager.DisableSubscriptionAsync(userId, subscriptionId, default));
		}

		[Fact]
		public async Task DisableAlreadyDisabledSubscription() {
			var subscriptionId = await CreateSubscription(new WebhookSubscriptionInfo("test.event", "https://callback.test.io/webhook") {
				Name = "Test Callback",
				Active = false
			});

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

			var result = await webhookManager.CountAllAsync(default);

			Assert.Equal(1, result);
		}

	}
}