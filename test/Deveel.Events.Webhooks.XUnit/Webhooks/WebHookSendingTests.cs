﻿using System;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

using Deveel.Util;

using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging.Abstractions;

using Mongo2Go;

using Xunit;

namespace Deveel.Webhooks {
	[Trait("Category", "Webhooks")]
	[Trait("Category", "Notification")]
	public class WebHookSendingTests : IDisposable {
		private MongoDbRunner mongoDbCluster;
		private readonly string tenantId = Guid.NewGuid().ToString();
		private IWebhookSubscriptionManager webhookManager;
		private IWebhookNotifier notifier;

		private bool validateSignature;
		private string webhookSecret;

		private WebhookPayload lastWebhook;
		private HttpResponseMessage testResponse;

		public WebHookSendingTests() {
			mongoDbCluster = MongoDbRunner.Start(logger: NullLogger.Instance);

			var services = new ServiceCollection();
			services.AddWebhooks(builder => {
				builder.ConfigureDelivery(options => {
					options.SignWebhooks = true;
				})
				.UseSubscriptionManager()
				.AddDynamicLinqFilterEvaluator()
				.AddDataFactory<TestDataFactory>()
				.UseMongoDb(options => {
					options.SubscriptionsCollectionName = "webhooks_subscription";
					options.DatabaseName = "webhooks";
					options.ConnectionString = mongoDbCluster.ConnectionString;
					options.MultiTenantHandling = MongoDbMultiTenancyHandling.TenantField;
					options.TenantField = "TenantId";
				});
			})
			.AddTestHttpClient(async request => {
				try {
					lastWebhook = await request.GetWebhookAsync<WebhookPayload>(new WebhookReceiveOptions {
						ValidateSignature = validateSignature,
						Secret = webhookSecret
					});

					if (testResponse != null)
						return testResponse;

					return new HttpResponseMessage(HttpStatusCode.Accepted);
				} catch (Exception) {
					return new HttpResponseMessage(HttpStatusCode.InternalServerError);
				}
			});

			var provider = services.BuildServiceProvider();

			webhookManager = provider.GetService<IWebhookSubscriptionManager>();
			notifier = provider.GetService<IWebhookNotifier>();
		}

		private Task<string> CreateSubscriptionAsync(string name, string eventType, params IWebhookFilter[] filters) {
			return CreateSubscriptionAsync(new WebhookSubscriptionInfo(eventType, "https://callback.example.com/webhook") {
				Name = name,
				RetryCount = 3,
				Filters = filters
			}, true);
		}

		private async Task<string> CreateSubscriptionAsync(WebhookSubscriptionInfo subscriptionInfo, bool enabled = true) {
			var id = await webhookManager.AddSubscriptionAsync(tenantId, Guid.NewGuid().ToString("N"), subscriptionInfo, CancellationToken.None);

			if (enabled)
				await webhookManager.EnableSubscriptionAsync(tenantId, Guid.NewGuid().ToString("N"), id, CancellationToken.None);

			return id;
		}

		[Fact]
		public async Task DeliverWebhookFromEvent() {
			var subscriptionId = await CreateSubscriptionAsync("Data Created", "data.created", new WebhookFilter("hook.data.data_type == \"test-data\"", "linq"));
			var notification = new EventInfo("data.created", new {
				creationTime = DateTimeOffset.UtcNow,
				type = "test"
			});

			var result = await notifier.NotifyAsync(tenantId, notification, CancellationToken.None);

			Assert.NotNull(result);
			Assert.NotEmpty(result);
			Assert.Equal(subscriptionId, result.First().Webhook.SubscriptionId);
			Assert.True(result[subscriptionId].Successful);
			Assert.Single(result[subscriptionId].Attempts);

			Assert.NotNull(lastWebhook);
			Assert.Equal("data.created", lastWebhook.EventType);
			Assert.Equal(notification.Id, lastWebhook.EventId);
			Assert.Equal(notification.TimeStamp, lastWebhook.TimeStamp);
		}

		[Fact]
		public async Task DeliverWebhookWithMultipleFiltersFromEvent() {
			var subscriptionId = await CreateSubscriptionAsync("Data Created", "data.created", 
				new WebhookFilter( "hook.data.data_type == \"test-data\"", "linq"), 
				new WebhookFilter("hook.data.creator.user_name == \"antonello\"", "linq"));
			var notification = new EventInfo("data.created", new {
				creationTime = DateTimeOffset.UtcNow,
				type = "test"
			});

			var result = await notifier.NotifyAsync(tenantId, notification, CancellationToken.None);

			Assert.NotNull(result);
			Assert.NotEmpty(result);
			Assert.Equal(subscriptionId, result.First().Webhook.SubscriptionId);
			Assert.True(result[subscriptionId].Successful);
			Assert.Single(result[subscriptionId].Attempts);

			Assert.NotNull(lastWebhook);
			Assert.Equal("data.created", lastWebhook.EventType);
			Assert.Equal(notification.Id, lastWebhook.EventId);
			Assert.Equal(notification.TimeStamp, lastWebhook.TimeStamp);
		}


		[Fact]
		public async Task DeliverWebhookWithoutFilter() {
			var subscriptionId = await CreateSubscriptionAsync("Data Created", "data.created", null);
			var notification = new EventInfo("data.created", new {
				creationTime = DateTimeOffset.UtcNow,
				type = "test"
			});

			var result = await notifier.NotifyAsync(tenantId, notification, CancellationToken.None);

			Assert.NotNull(result);
			Assert.NotEmpty(result);
			Assert.Equal(subscriptionId, result.First().Webhook.SubscriptionId);
			Assert.True(result[subscriptionId].Successful);
			Assert.Single(result[subscriptionId].Attempts);

			Assert.NotNull(lastWebhook);
			Assert.Equal("data.created", lastWebhook.EventType);
			Assert.Equal(notification.Id, lastWebhook.EventId);
			Assert.Equal(notification.TimeStamp, lastWebhook.TimeStamp);
		}

		[Fact]
		public async Task DeliverSignedWebhookFromEvent() {
			validateSignature = true;

			var subscriptionId = await CreateSubscriptionAsync(new WebhookSubscriptionInfo("data.created", "https://callback.example.com") {
				Filter = new WebhookFilter("hook.data.data_type == \"test-data\"", "linq"),
				Name = "Data Created",
				Secret = webhookSecret = "abc12345",
				RetryCount = 3
			});

			var notification = new EventInfo("data.created", new {
				creationTime = DateTimeOffset.UtcNow,
				type = "test"
			});

			var result = await notifier.NotifyAsync(tenantId, notification, CancellationToken.None);

			Assert.NotNull(result);
			Assert.NotEmpty(result);
			Assert.Equal(subscriptionId, result.First().Webhook.SubscriptionId);
			Assert.True(result[subscriptionId].Successful);
			Assert.Single(result[subscriptionId].Attempts);

			Assert.NotNull(lastWebhook);
			Assert.Equal("data.created", lastWebhook.EventType);
			Assert.Equal(notification.Id, lastWebhook.EventId);
			Assert.Equal(notification.TimeStamp, lastWebhook.TimeStamp);
		}

		[Fact]
		public async Task FailToDeliver() {
			var subscriptionId = await CreateSubscriptionAsync("Data Created", "data.created", new WebhookFilter ("hook.data.data_type == \"test-data\"", "linq"));
			var notification = new EventInfo("data.created", new { creationTime = DateTimeOffset.UtcNow, type = "test" });

			testResponse = new HttpResponseMessage(HttpStatusCode.InternalServerError);

			var result = await notifier.NotifyAsync(tenantId, notification, CancellationToken.None);

			Assert.NotNull(result);
			Assert.NotEmpty(result);
			Assert.Equal(subscriptionId, result.First().Webhook.SubscriptionId);
			Assert.False(result[subscriptionId].Successful);
			Assert.Equal(3, result[subscriptionId].Attempts.Count());
			Assert.Equal((int)HttpStatusCode.InternalServerError, result[subscriptionId].Attempts.ElementAt(0).ResponseStatusCode);
			Assert.Equal((int)HttpStatusCode.InternalServerError, result[subscriptionId].Attempts.ElementAt(1).ResponseStatusCode);
			Assert.Equal((int)HttpStatusCode.InternalServerError, result[subscriptionId].Attempts.ElementAt(2).ResponseStatusCode);
		}

		[Fact]
		public async Task NoSubscriptionMatches() {
			await CreateSubscriptionAsync("Data Created", "data.created",  new WebhookFilter("hook.data.data_type == \"test-data2\"", "linq"));
			var notification = new EventInfo("data.created", new { creationTime = DateTimeOffset.UtcNow, type = "test" });

			var result = await notifier.NotifyAsync(tenantId, notification, CancellationToken.None);

			Assert.NotNull(result);
			Assert.Empty(result);
		}

		[Fact]
		public async Task NoTenantMatches() {
			var subscriptionId = await CreateSubscriptionAsync("Data Created", "data.created", new WebhookFilter("hook.data.data_type == \"test-data\"", "linq"));
			var notification = new EventInfo("data.created", new { creationTime = DateTimeOffset.UtcNow, type = "test" });

			var result = await notifier.NotifyAsync(Guid.NewGuid().ToString("N"), notification, CancellationToken.None);

			Assert.NotNull(result);
			Assert.Empty(result);
		}

		[Fact]
		public async Task NoTenantSet() {
			var subscriptionId = await CreateSubscriptionAsync("Data Created", "data.created", new WebhookFilter("hook.data.data_type == \"test-data\"", "linq"));
			var notification = new EventInfo("data.created", new { creationTime = DateTimeOffset.UtcNow, type = "test" });

			await Assert.ThrowsAsync<ArgumentException>(() => notifier.NotifyAsync(null, notification, CancellationToken.None));
		}


		public void Dispose() {
			mongoDbCluster?.Dispose();
		}

		private class TestDataFactory : IWebhookDataFactory {
			public bool Handles(EventInfo eventInfo) => eventInfo.EventType == "data.created";

			public Task<object> CreateDataAsync(EventInfo eventInfo, CancellationToken cancellationToken)
				=> Task.FromResult<object>(new {
					creator = new { user_id = "1234", user_name = "antonello" },
					data_type = "test-data",
					created_at = DateTimeOffset.UtcNow
				});
		}
	}
}
