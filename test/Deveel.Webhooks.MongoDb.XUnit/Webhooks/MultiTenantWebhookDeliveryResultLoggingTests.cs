using System.Net;
using System.Net.Http.Json;

using Deveel.Data;

using Finbuckle.MultiTenant;

using Microsoft.Extensions.DependencyInjection;

using MongoDB.Bson;

using Xunit.Abstractions;

namespace Deveel.Webhooks {
	public class MultiTenantWebhookDeliveryResultLoggingTests : MongoDbWebhookTestBase {
		private const int TimeOutSeconds = 2;
		private bool testTimeout = false;

		private readonly string tenantId = Guid.NewGuid().ToString();

		private IWebhookSubscriptionRepositoryProvider<MongoWebhookSubscription, ObjectId> webhookStoreProvider;
		private IWebhookDeliveryResultRepositoryProvider<MongoWebhookDeliveryResult, ObjectId> deliveryResultStoreProvider;
		private ITenantWebhookNotifier<Webhook> notifier;

		private Webhook? lastWebhook;
		private HttpResponseMessage? testResponse;

		public MultiTenantWebhookDeliveryResultLoggingTests(MongoTestDatabase mongo, ITestOutputHelper outputHelper) 
			: base(mongo, outputHelper) {
			webhookStoreProvider = Services.GetRequiredService<IWebhookSubscriptionRepositoryProvider<MongoWebhookSubscription, ObjectId>>();
			deliveryResultStoreProvider = Services.GetRequiredService<IWebhookDeliveryResultRepositoryProvider<MongoWebhookDeliveryResult, ObjectId>>();
			notifier = Services.GetRequiredService<ITenantWebhookNotifier<Webhook>>();
		}

		protected override void ConfigureServices(IServiceCollection services) {
			services.AddMultiTenant<TenantInfo>()
				.WithInMemoryStore(store => {
					store.Tenants.Add(new TenantInfo {
						Id = tenantId,
						Identifier = tenantId,
						Name = "Test Tenant",
						ConnectionString = $"{ConnectionString}webhooks"
					});
				})
				.WithStaticStrategy(tenantId);

			services.AddWebhookNotifier<Webhook>(notifier => notifier
					.UseTenantNotifier()
					.UseSender(options => {
						options.Timeout = TimeSpan.FromSeconds(TimeOutSeconds);
						options.Retry.MaxRetries = 2;
					})
					.UseLinqFilter()
					.UseMongoTenantSubscriptionResolver()
					.UseMongoDeliveryResultLogger());

			base.ConfigureServices(services);
		}

		protected override void ConfigureWebhookService(WebhookSubscriptionBuilder<MongoWebhookSubscription, ObjectId> builder) {
			builder.UseSubscriptionManager()
				.UseMongoDb(options => options.UseMultiTenant());
		}

		private static MongoWebhook ConvertToMongo(Webhook webhook) {
			return new TestMongoWebhook {
				TimeStamp = webhook.TimeStamp,
				EventType = webhook.EventType,
				SubscriptionId = webhook.SubscriptionId!,
				EventName = webhook.Name!,
				WebhookId = webhook.Id,
			};
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
			}

			subscription.TenantId = tenantId;

			var store = webhookStoreProvider.GetRepository(tenantId);
			await store.AddAsync(subscription, default);

			return subscription.Id.ToString();
		}

		[Fact]
		public async Task DeliverWebhookFromEvent() {
			var subscriptionId = await CreateSubscriptionAsync("Data Created", "data.created");
			var notification = new EventInfo("test", "data.created", data: new {
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

			var store = await deliveryResultStoreProvider.GetRepositoryAsync(tenantId);
			var storedResult = await store.FindByWebhookIdAsync(webhookResult.Webhook.Id, default);

			Assert.NotNull(storedResult);
			Assert.NotNull(storedResult.Webhook);
			Assert.NotNull(storedResult.DeliveryAttempts);
			Assert.NotEmpty(storedResult.DeliveryAttempts);
		}

		[Fact]
		public async Task NotifyEventNotListened() {
			var notification = new EventInfo("test", "data.created", data: new {
				creationTime = DateTimeOffset.UtcNow,
				type = "test"
			});

			var result = await notifier.NotifyAsync(tenantId, notification, default);
			Assert.NotNull(result);
			Assert.Empty(result);
		}

		class TestMongoWebhook : MongoWebhook {
			public string SubscriptionId { get; set; }

			public string EventName { get; set; }
		}
	}
}
