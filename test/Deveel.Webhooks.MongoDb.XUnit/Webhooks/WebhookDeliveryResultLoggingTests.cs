using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;

using Deveel.Data;

using Microsoft.Extensions.DependencyInjection;

using Xunit;
using Xunit.Abstractions;

namespace Deveel.Webhooks {
	public class WebhookDeliveryResultLoggingTests : WebhookServiceTestBase {
		private const int TimeOutSeconds = 2;
		private bool testTimeout = false;

		private readonly string tenantId = Guid.NewGuid().ToString();

		private IWebhookSubscriptionStoreProvider<MongoDbWebhookSubscription> webhookStore;
		private IWebhookDeliveryResultStoreProvider<MongoDbWebhookDeliveryResult> deliveryResultStore;
		private IWebhookNotifier<Webhook> notifier;

		private Webhook lastWebhook;
		private HttpResponseMessage testResponse;

		public WebhookDeliveryResultLoggingTests(ITestOutputHelper outputHelper) : base(outputHelper) {
			webhookStore = Services.GetService<IWebhookSubscriptionStoreProvider<MongoDbWebhookSubscription>>();
			deliveryResultStore = Services.GetRequiredService<IWebhookDeliveryResultStoreProvider<MongoDbWebhookDeliveryResult>>();
			notifier = Services.GetService<IWebhookNotifier<Webhook>>();
		}

		protected override void ConfigureWebhookService(WebhookSubscriptionBuilder<MongoDbWebhookSubscription> builder) {
			builder
			.UseManager()
			.UseNotifier<Webhook>(notifier => notifier
				.UseSender(options => {
					options.Timeout = TimeSpan.FromSeconds(TimeOutSeconds);
					options.Retry.MaxRetries = 2;
				})
				.UseLinqFilter()
				.UseWebhookFactory<DefaultWebhookFactory>()
				.UseMongoSubscriptionResolver())
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

				lastWebhook = await httpRequest.Content.ReadFromJsonAsync<Webhook>();

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
			var notification = new EventInfo("test", "data.created", new {
				creationTime = DateTimeOffset.UtcNow,
				type = "test"
			});

			var result = await notifier.NotifyAsync(tenantId, notification, default);

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
			Assert.Equal(notification.TimeStamp.ToUnixTimeMilliseconds(), lastWebhook.TimeStamp.ToUnixTimeMilliseconds());

			var storedResult = await deliveryResultStore.FindByWebhookIdAsync(tenantId, webhookResult.Webhook.Id);

			Assert.NotNull(storedResult);
			Assert.NotNull(storedResult.Webhook);
			Assert.NotNull(storedResult.DeliveryAttempts);
			Assert.NotEmpty(storedResult.DeliveryAttempts);
		}

	}
}
