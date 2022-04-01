using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

using Deveel.Data;
using Deveel.Util;

using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;

using Mongo2Go;

using Newtonsoft.Json.Linq;

using Xunit;
using Xunit.Abstractions;

namespace Deveel.Webhooks {
	public class WebhookDeliveryResultLoggingTests : WebhookServiceTestBase {
		private const int TimeOutSeconds = 2;
		private bool testTimeout = false;

		private readonly string tenantId = Guid.NewGuid().ToString();

		private IWebhookSubscriptionStoreProvider<MongoDbWebhookSubscription> webhookStore;
		private IWebhookDeliveryResultStoreProvider<MongoDbWebhookDeliveryResult> deliveryResultStore;
		private IWebhookNotifier notifier;

		private WebhookPayload lastWebhook;
		private HttpResponseMessage testResponse;

		public WebhookDeliveryResultLoggingTests(ITestOutputHelper outputHelper) : base(outputHelper) {
			webhookStore = Services.GetService<IWebhookSubscriptionStoreProvider<MongoDbWebhookSubscription>>();
			deliveryResultStore = Services.GetRequiredService<IWebhookDeliveryResultStoreProvider<MongoDbWebhookDeliveryResult>>();
			notifier = Services.GetService<IWebhookNotifier>();
		}

		protected override void ConfigureWebhookService(WebhookServiceBuilder<MongoDbWebhookSubscription> builder) {
			builder.ConfigureDelivery(options =>
				options.SignWebhooks()
					   .SecondsBeforeTimeOut(TimeOutSeconds))
			.UseSubscriptionManager()
			.UseMongoDb(options => {
				options.DatabaseName = "webhooks";
				options.ConnectionString = ConnectionString;
				options.SubscriptionsCollectionName("webhooks_subscription");
				options.DeliveryResultsCollectionName("delivery_results");
				options.MultiTenancy.Handling = MongoDbMultiTenancyHandling.TenantField;
				options.MultiTenancy.TenantField = "TenantId";
			})
				.UseDeliveryResultLogger();
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
			return CreateSubscriptionAsync(new MongoDbWebhookSubscription {
				EventTypes = new List<string> { eventType },
				DestinationUrl = "https://callback.example.com/webhook",
				Name = name,
				RetryCount = 3,
				Filters = filters?.Select(x => new MongoDbWebhookFilter { Expression = x.Expression, Format = x.Format}).ToList()
			}, true);
		}

		private async Task<string> CreateSubscriptionAsync(MongoDbWebhookSubscription subscription, bool enabled = true) {
			if (enabled) {
				subscription.Status = WebhookSubscriptionStatus.Active;
				subscription.LastStatusTime = DateTime.Now;
			}

			return await webhookStore.CreateAsync(tenantId, subscription, default);
		}

		[Fact]
		public async Task DeliverWebhookFromEvent() {
			var subscriptionId = await CreateSubscriptionAsync("Data Created", "data.created");
			var notification = new EventInfo("data.created", new {
				creationTime = DateTimeOffset.UtcNow,
				type = "test"
			});

			var result = await notifier.NotifyAsync(tenantId, notification, default);

			Assert.NotNull(result);
			Assert.NotEmpty(result);
			Assert.True(result.HasSuccessful);
			Assert.False(result.HasFailed);
			Assert.NotEmpty(result.Successful);
			Assert.Empty(result.Failed);

			var webhookResult = result.First();

			Assert.Equal(subscriptionId, webhookResult.Webhook.SubscriptionId);
			Assert.True(result[subscriptionId].Successful);
			Assert.True(result[subscriptionId].HasAttempted);
			Assert.Single(result[subscriptionId].Attempts);
			Assert.NotNull(result[subscriptionId].LastAttempt);
			Assert.True(result[subscriptionId].LastAttempt.HasResponse);

			Assert.NotNull(lastWebhook);
			Assert.Equal("data.created", lastWebhook.EventType);
			Assert.Equal(notification.Id, lastWebhook.EventId);
			Assert.Equal(notification.TimeStamp, lastWebhook.TimeStamp);

			var storedResult = await deliveryResultStore.FindByWebhookIdAsync(tenantId, webhookResult.Webhook.Id);

			Assert.NotNull(storedResult);
			Assert.NotNull(storedResult.Webhook);
			Assert.NotNull(storedResult.DeliveryAttempts);
			Assert.NotEmpty(storedResult.DeliveryAttempts);
		}

	}
}
