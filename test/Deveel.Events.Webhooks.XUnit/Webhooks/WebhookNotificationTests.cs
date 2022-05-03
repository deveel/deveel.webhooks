using System;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

using Deveel.Data;
using Deveel.Webhooks;

using Microsoft.Extensions.DependencyInjection;

using MongoDB.Driver;

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
		private IWebhookNotifier notifier;

		private WebhookPayload lastWebhook;
		private HttpResponseMessage testResponse;

		public WebhookNotificationTests(ITestOutputHelper outputHelper) : base(outputHelper) {
			webhookManager = Services.GetService<IMultiTenantWebhookSubscriptionManager<MongoDbWebhookSubscription>>();
			notifier = Services.GetService<IWebhookNotifier>();
		}

		protected override void ConfigureWebhookService(WebhookServiceBuilder<MongoDbWebhookSubscription> builder) {
			builder.ConfigureDelivery(options =>
				options.SignWebhooks()
					   .SecondsBeforeTimeOut(TimeOutSeconds))
			.UseSubscriptionManager()
			.AddDataFactory<TestDataFactory>()
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
					await Task.Delay(TimeSpan.FromSeconds(TimeOutSeconds).Add(TimeSpan.FromSeconds(1)));
					return new HttpResponseMessage(HttpStatusCode.RequestTimeout);
				}

				var json = await httpRequest.Content.ReadAsStringAsync();
				lastWebhook = Newtonsoft.Json.JsonConvert.DeserializeObject<WebhookPayload>(json);

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
			Assert.True(result.HasSuccessful);
			Assert.False(result.HasFailed);
			Assert.NotEmpty(result.Successful);
			Assert.Empty(result.Failed);
			Assert.Equal(subscriptionId, result.First().Webhook.SubscriptionId);
			Assert.True(result[subscriptionId].Successful);
			Assert.True(result[subscriptionId].HasAttempted);
			Assert.Single(result[subscriptionId].Attempts);
			Assert.NotNull(result[subscriptionId].LastAttempt);
			Assert.True(result[subscriptionId].LastAttempt.HasResponse);

			Assert.NotNull(lastWebhook);
			Assert.Equal("data.created", lastWebhook.EventType);
			Assert.Equal(notification.Id, lastWebhook.EventId);
			Assert.Equal(notification.TimeStamp, lastWebhook.TimeStamp);

			var testData = Assert.IsType<JObject>(lastWebhook.Data);

			Assert.Equal("test-data", testData.Value<string>("data_type"));
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
			Assert.Equal(subscriptionId, result.First().Webhook.SubscriptionId);
			Assert.True(result[subscriptionId].HasAttempted);
			Assert.True(result[subscriptionId].Successful);
			Assert.Single(result[subscriptionId].Attempts);
			Assert.True(result[subscriptionId].LastAttempt.HasResponse);

			Assert.NotNull(lastWebhook);
			Assert.Equal("data.modified", lastWebhook.EventType);
			Assert.Equal(notification.Id, lastWebhook.EventId);
			Assert.Equal(notification.TimeStamp, lastWebhook.TimeStamp);

			var eventData = Assert.IsType<JObject>(lastWebhook.Data);

			Assert.Equal("test", eventData.Value<string>("type"));
			Assert.True(eventData.ContainsKey("creationTime"));
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
			Assert.NotEmpty(result.Failed);
			Assert.True(result.HasFailed);
			Assert.Equal(subscriptionId, result.First().Webhook.SubscriptionId);
			Assert.False(result[subscriptionId].Successful);
			Assert.Equal(3, result[subscriptionId].Attempts.Count());
			Assert.Equal((int)HttpStatusCode.InternalServerError, result[subscriptionId].Attempts.ElementAt(0).ResponseStatusCode);
			Assert.Equal((int)HttpStatusCode.InternalServerError, result[subscriptionId].Attempts.ElementAt(1).ResponseStatusCode);
			Assert.Equal((int)HttpStatusCode.InternalServerError, result[subscriptionId].Attempts.ElementAt(2).ResponseStatusCode);
		}

		[Fact]
		public async Task TimeOutWhileDelivering() {
			var subscriptionId = await CreateSubscriptionAsync("Data Created", "data.created", new WebhookFilter("hook.data.data_type == \"test-data\"", "linq"));
			var notification = new EventInfo("data.created", new { creationTime = DateTimeOffset.UtcNow, type = "test" });

			testTimeout = true;

			var result = await notifier.NotifyAsync(tenantId, notification, CancellationToken.None);

			Assert.NotNull(result);
			Assert.NotEmpty(result);
			Assert.Equal(subscriptionId, result.First().Webhook.SubscriptionId);
			Assert.False(result[subscriptionId].Successful);
			Assert.Equal(3, result[subscriptionId].Attempts.Count());
			Assert.Equal((int)HttpStatusCode.RequestTimeout, result[subscriptionId].Attempts.ElementAt(0).ResponseStatusCode);
			Assert.Equal((int)HttpStatusCode.RequestTimeout, result[subscriptionId].Attempts.ElementAt(1).ResponseStatusCode);
			Assert.Equal((int)HttpStatusCode.RequestTimeout, result[subscriptionId].Attempts.ElementAt(2).ResponseStatusCode);
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
