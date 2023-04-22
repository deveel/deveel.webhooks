using System;
using System.Collections.Generic;
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

using Mongo2Go;

using Xunit;
using Xunit.Abstractions;

namespace Deveel.Webhooks {
	[Trait("Category", "Webhooks")]
	[Trait("Category", "Notification")]
	public class WebhookNotificationTests : WebhookServiceTestBase {
		private const int TimeOutSeconds = 2;
		private bool testTimeout = false;

		private readonly string tenantId = Guid.NewGuid().ToString();

		private TestSubscriptionResolver subscriptionResolver;
		private IWebhookNotifier<Webhook> notifier;

		private Webhook lastWebhook;
		private HttpResponseMessage testResponse;

		protected override MongoDbRunner CreateMongo() => null;

		public WebhookNotificationTests(ITestOutputHelper outputHelper) : base(outputHelper) {
			notifier = Services.GetRequiredService<IWebhookNotifier<Webhook>>();
			subscriptionResolver = Services.GetRequiredService<TestSubscriptionResolver>();
		}

		protected override void ConfigureWebhookService(WebhookSubscriptionBuilder<MongoDbWebhookSubscription> builder) {
			builder
				.UseManager()
				.UseNotifier<Webhook>(config => config
					.UseWebhookFactory<DefaultWebhookFactory>()
					.AddDataTranformer<TestDataFactory>()
					.UseLinqFilter()
					.UseSubscriptionResolver<TestSubscriptionResolver>(ServiceLifetime.Singleton)
					.UseSender(options => {
						options.Retry.MaxRetries = 2;
						options.Retry.Timeout = TimeSpan.FromSeconds(TimeOutSeconds);
					}));
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

		private string CreateSubscription(string name, string eventType, params IWebhookFilter[] filters) {
			return CreateSubscription(new WebhookSubscriptionInfo(eventType, "https://callback.example.com/webhook") {
				Name = name,
				RetryCount = 3,
				Filters = filters
			}, true);
		}

		private string CreateSubscription(WebhookSubscriptionInfo subscriptionInfo, bool enabled = true) {
			var id = Guid.NewGuid().ToString();
			var subscription = new TestWebhookSubscription {
				SubscriptionId = id,
				Name = subscriptionInfo.Name,
				TenantId = tenantId,
				DestinationUrl = subscriptionInfo.DestinationUrl.ToString(),
				EventTypes = subscriptionInfo.EventTypes,
				Filters = subscriptionInfo.Filters,
				Status = enabled ? WebhookSubscriptionStatus.Active : WebhookSubscriptionStatus.Suspended,
				RetryCount = subscriptionInfo.RetryCount,
				Headers = subscriptionInfo.Headers,
				CreatedAt = DateTimeOffset.UtcNow,
				Secret = subscriptionInfo.Secret,
				Metadata = subscriptionInfo.Metadata
			};

			subscriptionResolver.AddSubscription(subscription);

			return id;
		}

		[Fact]
		public async Task DeliverWebhookFromEvent() {
			var subscriptionId = CreateSubscription("Data Created", "data.created", new WebhookFilter("hook.data.data_type == \"test-data\"", "linq"));
			var notification = new EventInfo("test", "data.created", new {
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
			var subscriptionId = CreateSubscription("Data Modified", "data.modified");
			var notification = new EventInfo("test", "data.modified", new {
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
			var subscriptionId = CreateSubscription("Data Created", "data.created", 
				new WebhookFilter( "hook.data.data_type == \"test-data\"", "linq"), 
				new WebhookFilter("hook.data.creator.user_name == \"antonello\"", "linq"));
			var notification = new EventInfo("test", "data.created", new {
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
			var subscriptionId = CreateSubscription("Data Created", "data.created", null);
			var notification = new EventInfo("test", "data.created", new {
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
			var subscriptionId = CreateSubscription(new WebhookSubscriptionInfo("data.created", "https://callback.example.com") {
				Filter = new WebhookFilter("hook.data.data_type == \"test-data\"", "linq"),
				Name = "Data Created",
				Secret = "abc12345",
				RetryCount = 3
			});

			var notification = new EventInfo("test", "data.created", new {
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
			var subscriptionId = CreateSubscription("Data Created", "data.created", new WebhookFilter ("hook.data.data_type == \"test-data\"", "linq"));
			var notification = new EventInfo("test", "data.created", new { 
				creationTime = DateTimeOffset.UtcNow, 
				type = "test"
			});

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
			var subscriptionId = CreateSubscription("Data Created", "data.created", new WebhookFilter("hook.data.data_type == \"test-data\"", "linq"));
			var notification = new EventInfo("test", "data.created", new { 
				creationTime = DateTimeOffset.UtcNow, 
				type = "test" 
			});

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
			CreateSubscription("Data Created", "data.created",  new WebhookFilter("hook.data.data_type == \"test-data2\"", "linq"));
			var notification = new EventInfo("test", "data.created", new { 
				creationTime = DateTimeOffset.UtcNow, 
				type = "test" 
			});

			var result = await notifier.NotifyAsync(tenantId, notification, CancellationToken.None);

			Assert.NotNull(result);
			Assert.Empty(result);
		}

		[Fact]
		public async Task NoTenantMatches() {
			var subscriptionId = CreateSubscription("Data Created", "data.created", new WebhookFilter("hook.data.data_type == \"test-data\"", "linq"));
			var notification = new EventInfo("test", "data.created", new { 
				creationTime = DateTimeOffset.UtcNow, 
				type = "test" 
			});

			var result = await notifier.NotifyAsync(Guid.NewGuid().ToString("N"), notification, CancellationToken.None);

			Assert.NotNull(result);
			Assert.Empty(result);
		}

		[Fact]
		public async Task NoTenantSet() {
			var subscriptionId = CreateSubscription("Data Created", "data.created", new WebhookFilter("hook.data.data_type == \"test-data\"", "linq"));
			var notification = new EventInfo("test", "data.created", new { 
				creationTime = DateTimeOffset.UtcNow, 
				type = "test" 
			});

			await Assert.ThrowsAsync<WebhookException>(() => notifier.NotifyAsync(null, notification, CancellationToken.None));
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

		private class TestWebhookSubscription : IWebhookSubscription {
			public string SubscriptionId { get; set; }

			public string TenantId { get; set; }

			public string Name { get; set; }

			public IEnumerable<string> EventTypes { get; set; }

			public string DestinationUrl { get; set; }

			public string Secret { get; set; }

			public WebhookSubscriptionStatus Status { get; set; }

			public int RetryCount { get; set; }

			public IEnumerable<IWebhookFilter> Filters { get; set; }

			public IDictionary<string, string> Headers { get; set; }

			public IDictionary<string, object> Metadata { get; set; }

			public DateTimeOffset? CreatedAt { get; set; }

			public DateTimeOffset? UpdatedAt { get; set; }
		}
	}
}
