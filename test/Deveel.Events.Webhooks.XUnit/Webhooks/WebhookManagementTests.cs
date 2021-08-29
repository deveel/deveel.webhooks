﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

using Deveel.Data;
using Deveel.Events;
using Deveel.Filters;

using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging.Abstractions;

using Mongo2Go;

using MongoDB.Bson;

using Xunit;

namespace Deveel.Webhooks {
	public class WebhookManagementTests : IDisposable {
		private readonly MongoDbRunner mongoDbCluster;
		private readonly string tenantId = Guid.NewGuid().ToString();
		private readonly IWebhookSubscriptionManager webhookManager;
		private readonly IStoreProvider<IWebhookSubscription> storeProvider;

		private readonly IWebhookSubscriptionFactory subscriptionFactory;

		public WebhookManagementTests() {
			mongoDbCluster = MongoDbRunner.Start(logger: NullLogger.Instance);

			var services = new ServiceCollection();
			services.AddWebhooks(buidler => {
				buidler.Configure(options => {
					options.Delivery.SignWebhooks = true;
				})
				.UseMongoDbStorage(options => {
					options.CollectionName = "webhooks_subscription";
					options.DatabaseName = "webhooks";
					options.ConnectionString = mongoDbCluster.ConnectionString;
				})
				.UseDefaultFilterEvaluator();
			})
				.AddTestHttpClient();

			var provider = services.BuildServiceProvider();

			webhookManager = provider.GetService<IWebhookSubscriptionManager>();
			storeProvider = provider.GetRequiredService<IWebhookSubscriptionStoreProvider>();
			subscriptionFactory = provider.GetRequiredService<IWebhookSubscriptionFactory>();
		}

		private async Task<string> CreateSubscription(WebhookSubscriptionInfo subscriptionInfo) {
			var subscription = subscriptionFactory.Create(subscriptionInfo);
			return await storeProvider.CreateAsync(tenantId, subscription);
		}

		[Fact]
		public async Task AddSubscription() {
			var subscriptionId = await webhookManager.AddSubscriptionAsync(tenantId, Guid.NewGuid().ToString("N"), new WebhookSubscriptionInfo("test.event", "https://callback.test.io/webhook"), CancellationToken.None);

			Assert.NotNull(subscriptionId);
			var subscription = await storeProvider.FindByIdAsync(tenantId, subscriptionId);

			Assert.NotNull(subscription);
			Assert.Contains("test.event", subscription.EventTypes);
		}

		[Fact]
		public async Task GetExistingSubscription() {
			var subscriptionId = await CreateSubscription(new WebhookSubscriptionInfo("test.event", "https://callback.test.io/webhook") {
				Filter = WebhookFilter.WildcardFilter ,
				Name = "Test Callback"
			});

			var subscription = await webhookManager.GetSubscriptionAsync(tenantId, subscriptionId, CancellationToken.None);

			Assert.NotNull(subscription);
			Assert.Equal(subscriptionId, subscription.Id);
			Assert.Contains("test.event", subscription.EventTypes);
			Assert.NotNull(subscription.Filters);
			Assert.NotEmpty(subscription.Filters);
			Assert.Single(subscription.Filters);
			Assert.True(subscription.Filters.First().IsWildcard());
		}

		[Fact]
		public async Task GetNotExistingSubscription() {
			var subscriptionId = ObjectId.GenerateNewId().ToString();
			var subscription = await webhookManager.GetSubscriptionAsync(tenantId, subscriptionId, CancellationToken.None);

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

			var result = await webhookManager.GetSubscriptionsAsync(tenantId, new PageRequest(1, 10), default);

			Assert.NotNull(result);
			Assert.NotEmpty(result.Items);
			Assert.Equal(2, result.TotalItems);
			Assert.Equal(1, result.TotalPages);
			Assert.Equal(subscriptionId1, result.Items.ElementAt(0).Id);
			Assert.Equal(subscriptionId2, result.Items.ElementAt(1).Id);
		}

		public void Dispose() {
			mongoDbCluster?.Dispose();
		}
	}
}
