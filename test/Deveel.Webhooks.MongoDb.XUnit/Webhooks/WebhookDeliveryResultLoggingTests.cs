using System.Net;
using System.Net.Http.Json;

using Finbuckle.MultiTenant;

using Microsoft.Extensions.DependencyInjection;

using Xunit.Abstractions;

namespace Deveel.Webhooks {
	public class WebhookDeliveryResultLoggingTests : MongoDbWebhookTestBase {
		private const int TimeOutSeconds = 2;
		private bool testTimeout = false;

		private readonly string tenantId = Guid.NewGuid().ToString();

		private IWebhookSubscriptionStoreProvider<MongoWebhookSubscription> webhookStoreProvider;
		private IWebhookDeliveryResultStoreProvider<MongoWebhookDeliveryResult> deliveryResultStoreProvider;
		private IWebhookNotifier<Webhook> notifier;

		private Webhook? lastWebhook;
		private HttpResponseMessage? testResponse;

		public WebhookDeliveryResultLoggingTests(MongoTestCluster mongo, ITestOutputHelper outputHelper) 
			: base(mongo, outputHelper) {
			webhookStoreProvider = Services.GetRequiredService<IWebhookSubscriptionStoreProvider<MongoWebhookSubscription>>();
			deliveryResultStoreProvider = Services.GetRequiredService<IWebhookDeliveryResultStoreProvider<MongoWebhookDeliveryResult>>();
			notifier = Services.GetRequiredService<IWebhookNotifier<Webhook>>();
		}

		protected override void ConfigureWebhookService(WebhookSubscriptionBuilder<MongoWebhookSubscription> builder) {
			builder.Services.AddMultiTenant<TenantInfo>()
				.WithInMemoryStore(store => {
					store.Tenants.Add(new TenantInfo {
						Id = tenantId,
						Identifier = tenantId,
						Name = "Test Tenant",
						ConnectionString = $"{ConnectionString}webhooks"
					});
				});

			builder
			.UseSubscriptionManager()
			.UseNotifier<Webhook>(notifier => notifier
				.UseSender(options => {
					options.Timeout = TimeSpan.FromSeconds(TimeOutSeconds);
					options.Retry.MaxRetries = 2;
				})
				.UseLinqFilter()
				.UseWebhookFactory<DefaultWebhookFactory>()
				.UseMongoSubscriptionResolver())
			.UseMongoDb(options => options
				.UseMultiTenant()
				.UseDeliveryResultLogger<Webhook>());
		}

		protected override async Task<HttpResponseMessage> OnRequestAsync(HttpRequestMessage httpRequest) {
			try {
				if (testTimeout) {
					await Task.Delay(TimeSpan.FromSeconds(TimeOutSeconds).Add(TimeSpan.FromSeconds(1)));
					return new HttpResponseMessage(HttpStatusCode.RequestTimeout);
				}

				lastWebhook = await httpRequest.Content!.ReadFromJsonAsync<Webhook>();

				if (testResponse != null)
					return testResponse;

				return new HttpResponseMessage(HttpStatusCode.Accepted);
			} catch (Exception) {
				return new HttpResponseMessage(HttpStatusCode.InternalServerError);
			}
		}

		private Task<string> CreateSubscriptionAsync(string name, string eventType, params IWebhookFilter[] filters) {
			return CreateSubscriptionAsync(new MongoWebhookSubscription {
				TenantId = tenantId,
				EventTypes = new List<string> { eventType },
				DestinationUrl = "https://callback.example.com/webhook",
				Name = name,
				RetryCount = 3,
				Filters = filters?.Select(x => new MongoWebhookFilter { Expression = x.Expression, Format = x.Format }).ToList()
				?? new List<MongoWebhookFilter>()
			}, true);
		}

		private async Task<string> CreateSubscriptionAsync(MongoWebhookSubscription subscription, bool enabled = true) {
			if (enabled) {
				subscription.Status = WebhookSubscriptionStatus.Active;
				subscription.LastStatusTime = DateTime.Now;
			}

			subscription.TenantId = tenantId;

			var store = webhookStoreProvider.GetTenantStore(tenantId);
			await store.CreateAsync(subscription, default);

			return subscription.Id.ToString();
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

			Assert.NotNull(result[subscriptionId]);
			Assert.Single(result[subscriptionId]!);

			var webhookResult = result[subscriptionId]![0];

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

			var store = deliveryResultStoreProvider.GetTenantStore(tenantId);
			var storedResult = await store.FindByWebhookIdAsync(webhookResult.Webhook.Id, default);

			Assert.NotNull(storedResult);
			Assert.NotNull(storedResult.Webhook);
			Assert.NotNull(storedResult.DeliveryAttempts);
			Assert.NotEmpty(storedResult.DeliveryAttempts);
		}

	}
}
