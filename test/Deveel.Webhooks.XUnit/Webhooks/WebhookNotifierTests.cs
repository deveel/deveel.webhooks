// Copyright 2022-2024 Antonello Provenzano
// 
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
// 
//     http://www.apache.org/licenses/LICENSE-2.0
// 
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.

using System.Net;
using System.Net.Http.Json;
using System.Text.Json;

using Microsoft.Extensions.DependencyInjection;

using Xunit;
using Xunit.Abstractions;

namespace Deveel.Webhooks {
	public class WebhookNotifierTests : WebhookServiceTestBase {
		private const int TimeOutSeconds = 2;
		private bool testTimeout = false;

		private TestSubscriptionResolver subscriptionResolver;
		private IWebhookNotifier<Webhook> notifier;

		private Webhook? lastWebhook;
		private HttpResponseMessage? testResponse;

		public WebhookNotifierTests(ITestOutputHelper outputHelper) : base(outputHelper) {
			notifier = Services.GetRequiredService<IWebhookNotifier<Webhook>>();
			subscriptionResolver = Services.GetRequiredService<TestSubscriptionResolver>();
		}

		protected override void ConfigureServices(IServiceCollection services) {
			services.AddWebhookNotifier<Webhook>(config => config
					.UseLinqFilter()
					.UseSubscriptionResolver<TestSubscriptionResolver>(ServiceLifetime.Singleton)
					.UseSender(options => {
						options.Retry.MaxRetries = 2;
						options.Retry.Timeout = TimeSpan.FromSeconds(TimeOutSeconds);
					}));

			base.ConfigureServices(services);
		}

		protected override async Task<HttpResponseMessage> OnRequestAsync(HttpRequestMessage httpRequest) {
			try {
				if (testTimeout) {
					await Task.Delay(TimeSpan.FromSeconds(TimeOutSeconds + 2));
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

			Assert.NotNull(lastWebhook);
			Assert.Equal("data.created", lastWebhook.EventType);
			Assert.Equal(eventInfo.Id, lastWebhook.Id);
			Assert.Equal(eventInfo.TimeStamp.ToUnixTimeSeconds(), lastWebhook.TimeStamp.ToUnixTimeSeconds());

			var testData = Assert.IsType<JsonElement>(lastWebhook.Data);

			Assert.Equal("test", testData.GetProperty("type").GetString());
		}

		[Fact]
		public async Task DeliverWebhookFromMultipleEvents() {
			var subscriptionId = CreateSubscription("Data Created", "data.created", new WebhookFilter("hook.data[0].type.equals(\"test\")", "linq"));
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
			Assert.Equal(notification.NotificationId, lastWebhook.Id);
			// TODO: how to determine the timestamp of the notification that has multiple events?
			// Assert.Equal(notification.TimeStamp.ToUnixTimeSeconds(), lastWebhook.TimeStamp.ToUnixTimeSeconds());

			var testData = Assert.IsType<JsonElement>(lastWebhook.Data);

			Assert.Equal(JsonValueKind.Array, testData.ValueKind);
			Assert.Equal(2, testData.GetArrayLength());
			Assert.Equal("test", testData[0].GetProperty("type").GetString());
		}


		[Fact]
		public async Task DeliverWebhookFromEvent_NoTransformations() {
			var subscriptionId = CreateSubscription("Data Modified", "data.modified");
			var notification = new EventInfo("test", "data.modified", data: new {
				creationTime = DateTimeOffset.UtcNow,
				type = "test"
			});

			var result = await notifier.NotifyAsync(notification, CancellationToken.None);

			Assert.NotNull(result);
			Assert.NotEmpty(result);
			Assert.True(result.HasSuccessful);
			Assert.False(result.HasFailed);
			Assert.NotEmpty(result.Successful);
			Assert.Empty(result.Failed);

			Assert.Single(result[subscriptionId]!);

			var webhookResult = result[subscriptionId]![0];

			Assert.Equal(subscriptionId, webhookResult.Webhook.SubscriptionId);
			Assert.True(webhookResult.HasAttempted);
			Assert.True(webhookResult.Successful);
			Assert.Single(webhookResult.Attempts);
			Assert.NotNull(webhookResult.LastAttempt);
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
				new WebhookFilter("hook.data.type == \"test\"", "linq"),
				new WebhookFilter("hook.data.creator.user_name == \"antonello\"", "linq"));
			var notification = new EventInfo("test", "data.created", data: new {
				creator = new {
					user_name = "antonello"
				},
				creationTime = DateTimeOffset.UtcNow,
				type = "test"
			});

			var result = await notifier.NotifyAsync(notification, CancellationToken.None);

			Assert.NotNull(result);
			Assert.NotEmpty(result);
			Assert.Single(result);

			Assert.NotNull(result[subscriptionId]);
			Assert.Single(result[subscriptionId]!);

			var webhookResult = result[subscriptionId]![0];

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
			var subscriptionId = CreateSubscription("Data Created", "data.created");
			var notification = new EventInfo("test", "data.created", data: new {
				creationTime = DateTimeOffset.UtcNow,
				type = "test"
			});

			var result = await notifier.NotifyAsync(notification, CancellationToken.None);

			Assert.NotNull(result);
			Assert.NotEmpty(result);
			Assert.Single(result);

			Assert.NotNull(result[subscriptionId]);
			Assert.Single(result[subscriptionId]!);

			var webhookResult = result[subscriptionId]![0];

			Assert.Equal(subscriptionId, webhookResult.Webhook.SubscriptionId);
			Assert.True(webhookResult.Successful);
			Assert.Single(webhookResult.Attempts);

			Assert.NotNull(lastWebhook);
			Assert.Equal("data.created", lastWebhook.EventType);
			Assert.Equal(notification.Id, lastWebhook.Id);
			Assert.Equal(notification.TimeStamp.ToUnixTimeMilliseconds(), lastWebhook.TimeStamp.ToUnixTimeMilliseconds());
		}

		[Fact]
		public async Task DeliverWenhookWithFilterTypeNotRegistered() {
			var subscriptionId = CreateSubscription("Data Created", "data.created",
						new WebhookFilter("hook.data.type == \"test\"", "query"),
						new WebhookFilter("hook.data.creator.user_name == \"antonello\"", "query"));

			var notification = new EventInfo("test", "data.created", data: new {
				creator = new {
					user_name = "antonello"
				},
				creationTime = DateTimeOffset.UtcNow,
				type = "test"
			});

			var result = await notifier.NotifyAsync(notification, CancellationToken.None);
			Assert.NotNull(result);
			Assert.Empty(result);
		}

		[Fact]
		public async Task DeliverSignedWebhookFromEvent() {
			var subscriptionId = CreateSubscription(new TestWebhookSubscription {
				EventTypes = new[] { "data.created" },
				DestinationUrl = "https://callback.example.com",
				Filters = new[] { new WebhookFilter("hook.data.type == \"test\"", "linq") },
				Name = "Data Created",
				Secret = "abc12345",
				RetryCount = 3,
				Status = WebhookSubscriptionStatus.Active
			});

			var notification = new EventInfo("test", "data.created", data: new {
				creationTime = DateTimeOffset.UtcNow,
				type = "test"
			});

			var result = await notifier.NotifyAsync(notification, CancellationToken.None);

			Assert.NotNull(result);
			Assert.NotEmpty(result);
			Assert.Single(result);

			Assert.NotNull(result[subscriptionId]);
			Assert.Single(result[subscriptionId]!);

			var webhookResult = result[subscriptionId]![0];

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
			var subscriptionId = CreateSubscription("Data Created", "data.created", new WebhookFilter("hook.data.type == \"test\"", "linq"));
			var notification = new EventInfo("test", "data.created", data: new {
				creationTime = DateTimeOffset.UtcNow,
				type = "test"
			});

			testResponse = new HttpResponseMessage(HttpStatusCode.InternalServerError);

			var result = await notifier.NotifyAsync(notification, CancellationToken.None);

			Assert.NotNull(result);
			Assert.NotEmpty(result);
			Assert.Single(result);
			Assert.NotEmpty(result.Failed);
			Assert.True(result.HasFailed);

			Assert.NotNull(result[subscriptionId]);
			Assert.Single(result[subscriptionId]!);

			var webhookResult = result[subscriptionId]![0];

			Assert.Equal(subscriptionId, webhookResult.Webhook.SubscriptionId);
			Assert.False(webhookResult.Successful);
			Assert.Equal(3, webhookResult.Attempts.Count);
			Assert.Equal((int)HttpStatusCode.InternalServerError, webhookResult.Attempts[0].ResponseCode);
		}

		[Fact]
		public async Task TimeOutWhileDelivering() {
			var subscriptionId = CreateSubscription("Data Created", "data.created", new WebhookFilter("hook.data.type == \"test\"", "linq"));
			var notification = new EventInfo("test", "data.created", data: new {
				creationTime = DateTimeOffset.UtcNow,
				type = "test"
			});

			testTimeout = true;

			var result = await notifier.NotifyAsync(notification, CancellationToken.None);

			Assert.NotNull(result);
			Assert.NotEmpty(result);
			Assert.Single(result);

			Assert.NotNull(result[subscriptionId]);
			Assert.Single(result[subscriptionId]!);

			var webhookResult = result[subscriptionId]![0];

			Assert.Equal(subscriptionId, webhookResult.Webhook.SubscriptionId);
			Assert.False(webhookResult.Successful);
			Assert.Equal(3, webhookResult.Attempts.Count);
			Assert.Equal((int)HttpStatusCode.RequestTimeout, webhookResult.Attempts.ElementAt(0).ResponseCode);
		}



		[Fact]
		public async Task NoSubscriptionMatches() {
			CreateSubscription("Data Created", "data.created", new WebhookFilter("hook.data.type == \"test-data2\"", "linq"));
			var notification = new EventInfo("test", "data.created", data: new {
				creationTime = DateTimeOffset.UtcNow,
				type = "test"
			});

			var result = await notifier.NotifyAsync(notification, CancellationToken.None);

			Assert.NotNull(result);
			Assert.Empty(result);
		}
	}
}
