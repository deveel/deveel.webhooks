using System.Net.Http.Json;
using System.Net;

using Microsoft.Extensions.DependencyInjection;

using Xunit.Abstractions;
using System.Text.Json;
using Xunit;

namespace Deveel.Webhooks {
	public class NotifyOneWebhookPerEventTests : WebhookServiceTestBase {
		private TestSubscriptionResolver subscriptionResolver;
		private IWebhookNotifier<Webhook> notifier;

		private IList<Webhook> lastWebhooks;
		private HttpResponseMessage? testResponse;

		public NotifyOneWebhookPerEventTests(ITestOutputHelper outputHelper) : base(outputHelper) {
			notifier = Services.GetRequiredService<IWebhookNotifier<Webhook>>();
			subscriptionResolver = Services.GetRequiredService<TestSubscriptionResolver>();
		}

		protected override void ConfigureServices(IServiceCollection services) {
			services.AddWebhookNotifier<Webhook>(notifier => notifier
				.UseWebhookFactory(options => options.CreateStrategy = WebhookCreateStrategy.OnePerEvent)
				.UseLinqFilter()
				.UseSubscriptionResolver<TestSubscriptionResolver>(ServiceLifetime.Singleton)
				.UseSender());

			base.ConfigureServices(services);
		}

		protected override async Task<HttpResponseMessage> OnRequestAsync(HttpRequestMessage httpRequest) {
			try {
				if (lastWebhooks == null)
					lastWebhooks = new List<Webhook>();

				var webhook = await httpRequest.Content!.ReadFromJsonAsync<Webhook>();
				if (webhook != null)
					lastWebhooks.Add(webhook);

				return new HttpResponseMessage(HttpStatusCode.Accepted);
			} catch (Exception) {
				return new HttpResponseMessage(HttpStatusCode.InternalServerError);
			}
		}

		private string CreateSubscription(string name, string eventType, params WebhookFilter[] filters) {
			return CreateSubscription(new TestWebhookSubscription {
				EventTypes = new[] { eventType },
				DestinationUrl = "https://callback.example.com/webhook",
				Name = name,
				RetryCount = 3,
				Filters = filters,
				Status = WebhookSubscriptionStatus.Active,
				CreatedAt = DateTimeOffset.UtcNow
			}, true);
		}

		private string CreateSubscription(TestWebhookSubscription subscription, bool enabled = true) {
			var id = Guid.NewGuid().ToString();

			subscription.SubscriptionId = id;

			subscriptionResolver.AddSubscription(subscription);

			return id;
		}

		[Fact]
		public async Task DeliverWebhookFromSingleEvent() {
			var subscriptionId = CreateSubscription("Data Created", "data.created", new WebhookFilter("hook.data.type == \"test\"", "linq"));
			var eventInfo = new EventInfo("test", "data.created", data: new {
				creationTime = DateTimeOffset.UtcNow,
				type = "test"
			});

			var result = await notifier.NotifyAsync(eventInfo, CancellationToken.None);

			Assert.NotNull(result);
			Assert.NotEmpty(result);
			Assert.Single(result);
			Assert.True(result.HasSuccessful);
			Assert.False(result.HasFailed);
			Assert.NotEmpty(result.Successful);
			Assert.Empty(result.Failed);

			Assert.Single(result[subscriptionId]!);

			var webhookResult = result[subscriptionId]![0];

			Assert.Equal(subscriptionId, webhookResult.Webhook.SubscriptionId);
			Assert.True(webhookResult.Successful);
			Assert.True(webhookResult.HasAttempted);
			Assert.Single(webhookResult.Attempts);
			Assert.NotNull(webhookResult.LastAttempt);
			Assert.True(webhookResult.LastAttempt.HasResponse);

			Assert.NotNull(lastWebhooks);
			Assert.Single(lastWebhooks);
			Assert.Equal("data.created", lastWebhooks[0].EventType);
			Assert.Equal(eventInfo.Id, lastWebhooks[0].Id);
			Assert.Equal(eventInfo.TimeStamp.ToUnixTimeSeconds(), lastWebhooks[0].TimeStamp.ToUnixTimeSeconds());

			var testData = Assert.IsType<JsonElement>(lastWebhooks[0].Data);

			Assert.Equal("test", testData.GetProperty("type").GetString());
		}

		[Fact]
		public async Task DeliverWebhookFromMultipleEvents() {
			var subscriptionId = CreateSubscription("Data Created", "data.created", new WebhookFilter("hook.data.type.startsWith(\"test\")", "linq"));
			EventNotification notification = new[] {
				new EventInfo("test", "data.created", data: new {
					creationTime = DateTimeOffset.UtcNow,
					type = "test"
				}),
				new EventInfo("test", "data.created", data: new {
					creationTime = DateTimeOffset.UtcNow.AddSeconds(3),
					type = "test2"
				})
			};

			var result = await notifier.NotifyAsync(notification, CancellationToken.None);

			Assert.NotNull(result);
			Assert.NotEmpty(result);
			Assert.Single(result);
			Assert.True(result.HasSuccessful);
			Assert.False(result.HasFailed);
			Assert.NotEmpty(result.Successful);
			Assert.Empty(result.Failed);

			Assert.Equal(2, result[subscriptionId]!.Count);

			var webhookResult = result[subscriptionId]![0];

			Assert.Equal(subscriptionId, webhookResult.Webhook.SubscriptionId);
			Assert.True(webhookResult.Successful);
			Assert.True(webhookResult.HasAttempted);
			Assert.Single(webhookResult.Attempts);
			Assert.NotNull(webhookResult.LastAttempt);
			Assert.True(webhookResult.LastAttempt.HasResponse);

			Assert.NotNull(lastWebhooks);
			Assert.Equal(2, lastWebhooks.Count);
			Assert.Equal("data.created", lastWebhooks[0].EventType);
			Assert.Equal(notification.Events[0].Id, lastWebhooks[0].Id);
			// TODO: how to determine the timestamp of the notification that has multiple events?
			// Assert.Equal(notification.TimeStamp.ToUnixTimeSeconds(), lastWebhook.TimeStamp.ToUnixTimeSeconds());

			var testData = Assert.IsType<JsonElement>(lastWebhooks[0].Data);

			Assert.Equal("test", testData.GetProperty("type").GetString());
		}

	}
}
