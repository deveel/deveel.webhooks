using System;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Json;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;

using Deveel.Data;
using Deveel.Webhooks;

using Microsoft.Extensions.DependencyInjection;

using MongoDB.Driver;

using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

using Xunit;
using Xunit.Abstractions;

namespace Deveel.Webhooks {
	[Trait("Category", "Webhooks")]
	[Trait("Category", "Notification")]
	public class WebhookNotificationTests : WebhookServiceTestBase {
		private const int TimeOutSeconds = 2;
		private bool testTimeout = false;

		private readonly string tenantId = Guid.NewGuid().ToString();

		private IMultiTenantWebhookSubscriptionManager<MongoDbWebhookSubscription> webhookManager;
		private IWebhookNotifier<Webhook> notifier;

		private Webhook lastWebhook;
		private HttpResponseMessage testResponse;

		public WebhookNotificationTests(ITestOutputHelper outputHelper) : base(outputHelper) {
			webhookManager = Services.GetService<IMultiTenantWebhookSubscriptionManager<MongoDbWebhookSubscription>>();
			notifier = Services.GetService<IWebhookNotifier<Webhook>>();
		}

		protected override void ConfigureWebhookService(WebhookSubscriptionBuilder<MongoDbWebhookSubscription> builder) {
			builder
				.UseManager()
				.UseNotifier<Webhook>(config => config
					.UseFactory<DefaultWebhookFactory>()
					.AddDataTranformer<TestDataFactory>()
					.UseLinqFilter()
					.UseMongoSubscriptionResolver()
					.UseSender(options => {
						options.Retry.MaxRetries = 2;
						options.Retry.Timeout = TimeSpan.FromSeconds(TimeOutSeconds);
					}))
				.UseMongoDb(options => {
					options.DatabaseName = "webhooks";
					options.ConnectionString = ConnectionString;
					options.SubscriptionsCollectionName("webhooks_subscription");
					options.MultiTenancy.Handling = MongoDbMultiTenancyHandling.TenantField;
					options.MultiTenancy.TenantField = "TenantId";
				});
		}

		protected override async Task<HttpResponseMessage> OnRequestAsync(HttpRequestMessage httpRequest) {
			try {
				if (testTimeout) {
					await Task.Delay(TimeSpan.FromSeconds(TimeOutSeconds + 2));
					return new HttpResponseMessage(HttpStatusCode.RequestTimeout);
				}

				lastWebhook = await httpRequest.Content.ReadFromJsonAsync<Webhook>();

				if (testResponse != null)
					return testResponse;

				return new HttpResponseMessage(HttpStatusCode.Accepted);
			} catch (Exception) {
				return new HttpResponseMessage(HttpStatusCode.InternalServerError);
			}
		}

		private Task<string> CreateSubscriptionAsync(string name, string eventType, params IWebhookFilter[] filters) {
			return CreateSubscriptionAsync(new WebhookSubscriptionInfo(eventType, "https://callback.example.com/webhook") {
				Name = name,
				RetryCount = 3,
				Filters = filters
			}, true);
		}

		private async Task<string> CreateSubscriptionAsync(WebhookSubscriptionInfo subscriptionInfo, bool enabled = true) {
			var id = await webhookManager.AddSubscriptionAsync(tenantId, subscriptionInfo, CancellationToken.None);

			if (enabled)
				await webhookManager.EnableSubscriptionAsync(tenantId, id, CancellationToken.None);

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
			Assert.Single(result);
			Assert.True(result.HasSuccessful);
			Assert.False(result.HasFailed);
			Assert.NotEmpty(result.Successful);
			Assert.Empty(result.Failed);

			Assert.Single(result[subscriptionId]);

			var webhookResult = result[subscriptionId][0];

			Assert.Equal(subscriptionId, webhookResult.Webhook.SubscriptionId);
			Assert.True(webhookResult.Successful);
			Assert.True(webhookResult.HasAttempted);
			Assert.Single(webhookResult.Attempts);
			Assert.NotNull(webhookResult.LastAttempt);
			Assert.True(webhookResult.LastAttempt.HasResponse);

			Assert.NotNull(lastWebhook);
			Assert.Equal("data.created", lastWebhook.EventType);
			Assert.Equal(notification.Id, lastWebhook.Id);
			Assert.Equal(notification.TimeStamp.ToUnixTimeSeconds(), lastWebhook.TimeStamp.ToUnixTimeSeconds());

			var testData = Assert.IsType<JsonElement>(lastWebhook.Data);

			Assert.Equal("test-data", testData.GetProperty("data_type").GetString());
		}

		[Fact]
		public async Task DeliverWebhookFromEvent_NoTransformations() {
			var subscriptionId = await CreateSubscriptionAsync("Data Modified", "data.modified");
			var notification = new EventInfo("data.modified", new {
				creationTime = DateTimeOffset.UtcNow,
				type = "test"
			});

			var result = await notifier.NotifyAsync(tenantId, notification, CancellationToken.None);

			Assert.NotNull(result);
			Assert.NotEmpty(result);
			Assert.True(result.HasSuccessful);
			Assert.False(result.HasFailed);
			Assert.NotEmpty(result.Successful);
			Assert.Empty(result.Failed);

			Assert.Single(result[subscriptionId]);

			var webhookResult = result[subscriptionId][0];

			Assert.Equal(subscriptionId, webhookResult.Webhook.SubscriptionId);
			Assert.True(webhookResult.HasAttempted);
			Assert.True(webhookResult.Successful);
			Assert.Single(webhookResult.Attempts);
			Assert.True(webhookResult.LastAttempt.HasResponse);

			Assert.NotNull(lastWebhook);
			Assert.Equal("data.modified", lastWebhook.EventType);
			Assert.Equal(notification.Id, lastWebhook.Id);
			Assert.Equal(notification.TimeStamp.ToUnixTimeMilliseconds(), lastWebhook.TimeStamp.ToUnixTimeMilliseconds());

			var eventData = Assert.IsType<JsonElement>(lastWebhook.Data);

			Assert.Equal("test", eventData.GetProperty("type").GetString());
			Assert.True(eventData.TryGetProperty("creationTime", out var creationTime));
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
			Assert.Single(result);

			Assert.Single(result[subscriptionId]);

			var webhookResult = result[subscriptionId][0];

			Assert.Equal(subscriptionId, webhookResult.Webhook.SubscriptionId);
			Assert.True(webhookResult.Successful);
			Assert.Single(webhookResult.Attempts);

			Assert.NotNull(lastWebhook);
			Assert.Equal("data.created", lastWebhook.EventType);
			Assert.Equal(notification.Id, lastWebhook.Id);
			Assert.Equal(notification.TimeStamp.ToUnixTimeMilliseconds(), lastWebhook.TimeStamp.ToUnixTimeMilliseconds());
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
			Assert.Single(result);

			Assert.Single(result[subscriptionId]);

			var webhookResult = result[subscriptionId][0];

			Assert.Equal(subscriptionId, webhookResult.Webhook.SubscriptionId);
			Assert.True(webhookResult.Successful);
			Assert.Single(webhookResult.Attempts);

			Assert.NotNull(lastWebhook);
			Assert.Equal("data.created", lastWebhook.EventType);
			Assert.Equal(notification.Id, lastWebhook.Id);
			Assert.Equal(notification.TimeStamp.ToUnixTimeMilliseconds(), lastWebhook.TimeStamp.ToUnixTimeMilliseconds());
		}

		[Fact]
		public async Task DeliverSignedWebhookFromEvent() {
			var subscriptionId = await CreateSubscriptionAsync(new WebhookSubscriptionInfo("data.created", "https://callback.example.com") {
				Filter = new WebhookFilter("hook.data.data_type == \"test-data\"", "linq"),
				Name = "Data Created",
				Secret = "abc12345",
				RetryCount = 3
			});

			var notification = new EventInfo("data.created", new {
				creationTime = DateTimeOffset.UtcNow,
				type = "test"
			});

			var result = await notifier.NotifyAsync(tenantId, notification, CancellationToken.None);

			Assert.NotNull(result);
			Assert.NotEmpty(result);
			Assert.Single(result);

			Assert.Single(result[subscriptionId]);

			var webhookResult = result[subscriptionId][0];

			Assert.Equal(subscriptionId, webhookResult.Webhook.SubscriptionId);
			Assert.True(webhookResult.Successful);
			Assert.Single(webhookResult.Attempts);

			Assert.NotNull(lastWebhook);
			Assert.Equal("data.created", lastWebhook.EventType);
			Assert.Equal(notification.Id, lastWebhook.Id);
			Assert.Equal(notification.TimeStamp.ToUnixTimeMilliseconds(), lastWebhook.TimeStamp.ToUnixTimeMilliseconds());
		}

		[Fact]
		public async Task FailToDeliver() {
			var subscriptionId = await CreateSubscriptionAsync("Data Created", "data.created", new WebhookFilter ("hook.data.data_type == \"test-data\"", "linq"));
			var notification = new EventInfo("data.created", new { creationTime = DateTimeOffset.UtcNow, type = "test" });

			testResponse = new HttpResponseMessage(HttpStatusCode.InternalServerError);

			var result = await notifier.NotifyAsync(tenantId, notification, CancellationToken.None);

			Assert.NotNull(result);
			Assert.NotEmpty(result);
			Assert.Single(result);
			Assert.NotEmpty(result.Failed);
			Assert.True(result.HasFailed);

			Assert.Single(result[subscriptionId]);

			var webhookResult = result[subscriptionId][0];

			Assert.Equal(subscriptionId, webhookResult.Webhook.SubscriptionId);
			Assert.False(webhookResult.Successful);
			Assert.Equal(3, webhookResult.Attempts.Count);
			Assert.Equal((int)HttpStatusCode.InternalServerError, webhookResult.Attempts[0].ResponseCode);
		}

		[Fact]
		public async Task TimeOutWhileDelivering() {
			var subscriptionId = await CreateSubscriptionAsync("Data Created", "data.created", new WebhookFilter("hook.data.data_type == \"test-data\"", "linq"));
			var notification = new EventInfo("data.created", new { creationTime = DateTimeOffset.UtcNow, type = "test" });

			testTimeout = true;

			var result = await notifier.NotifyAsync(tenantId, notification, CancellationToken.None);

			Assert.NotNull(result);
			Assert.NotEmpty(result);
			Assert.Single(result);

			Assert.Single(result[subscriptionId]);

			var webhookResult = result[subscriptionId][0];

			Assert.Equal(subscriptionId, webhookResult.Webhook.SubscriptionId);
			Assert.False(webhookResult.Successful);
			Assert.Equal(3, webhookResult.Attempts.Count);
			Assert.Equal((int)HttpStatusCode.RequestTimeout, webhookResult.Attempts.ElementAt(0).ResponseCode);
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
