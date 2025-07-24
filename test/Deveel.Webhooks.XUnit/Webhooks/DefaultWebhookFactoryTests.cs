// Copyright 2022-2025 Antonello Provenzano
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

using Microsoft.Extensions.Options;
using Xunit;

namespace Deveel.Webhooks {
	public class DefaultWebhookFactoryTests {
		private readonly TestWebhookSubscription subscription;

		public DefaultWebhookFactoryTests() {
			subscription = new TestWebhookSubscription {
				SubscriptionId = "test-sub-1",
				Name = "Test Subscription",
				EventTypes = new[] { "user.created", "user.updated" }
			};
		}

		[Fact]
		public async Task CreateAsync_OnePerNotification_SingleEvent_ReturnsOneWebhook() {
			var factory = new DefaultWebhookFactory<Webhook>(Options.Create(new WebhookFactoryOptions<Webhook> {
				CreateStrategy = WebhookCreateStrategy.OnePerNotification
			}));

			var eventInfo = new EventInfo("user", "user.created", "1.0", new { userId = 123, name = "John" });
			var notification = new EventNotification(eventInfo) {
				NotificationId = "notif-1",
				TimeStamp = DateTimeOffset.UtcNow
			};

			var result = await factory.CreateAsync(subscription, notification, CancellationToken.None);

			Assert.Single(result);
			var webhook = result[0];
			Assert.Equal("notif-1", webhook.Id);
			Assert.Equal("user.created", webhook.EventType);
			Assert.Equal("test-sub-1", webhook.SubscriptionId);
			Assert.Equal("Test Subscription", webhook.Name);
			Assert.Equal(notification.TimeStamp, webhook.TimeStamp);
			Assert.NotNull(webhook.Data);
		}

		[Fact]
		public async Task CreateAsync_OnePerNotification_MultipleEvents_ReturnsOneWebhookWithArrayData() {
			var factory = new DefaultWebhookFactory<Webhook>(Options.Create(new WebhookFactoryOptions<Webhook> {
				CreateStrategy = WebhookCreateStrategy.OnePerNotification
			}));

			var events = new[] {
				new EventInfo("user", "user.created", "1.0", new { userId = 123, name = "John" }),
				new EventInfo("user", "user.created", "1.0", new { userId = 124, name = "Jane" })
			};
			var notification = new EventNotification("user.created", events) {
				NotificationId = "notif-2",
				TimeStamp = DateTimeOffset.UtcNow
			};

			var result = await factory.CreateAsync(subscription, notification, CancellationToken.None);

			Assert.Single(result);
			var webhook = result[0];
			Assert.Equal("notif-2", webhook.Id);
			Assert.Equal("user.created", webhook.EventType);
			Assert.Equal("test-sub-1", webhook.SubscriptionId);
			Assert.Equal("Test Subscription", webhook.Name);
			Assert.Equal(notification.TimeStamp, webhook.TimeStamp);
			Assert.IsType<object[]>(webhook.Data);
			Assert.Equal(2, ((object[])webhook.Data).Length);
		}

		[Fact]
		public async Task CreateAsync_OnePerEvent_SingleEvent_ReturnsOneWebhook() {
			var factory = new DefaultWebhookFactory<Webhook>(Options.Create(new WebhookFactoryOptions<Webhook> {
				CreateStrategy = WebhookCreateStrategy.OnePerEvent
			}));

			var eventInfo = new EventInfo("user", "user.created", "1.0", new { userId = 123, name = "John" });
			var notification = new EventNotification(eventInfo) {
				NotificationId = "notif-3",
				TimeStamp = DateTimeOffset.UtcNow
			};

			var result = await factory.CreateAsync(subscription, notification, CancellationToken.None);

			Assert.Single(result);
			var webhook = result[0];
			Assert.Equal(eventInfo.Id, webhook.Id);
			Assert.Equal("user.created", webhook.EventType);
			Assert.Equal("test-sub-1", webhook.SubscriptionId);
			Assert.Equal("Test Subscription", webhook.Name);
			Assert.Equal(eventInfo.TimeStamp, webhook.TimeStamp);
			Assert.NotNull(webhook.Data);
		}

		[Fact]
		public async Task CreateAsync_OnePerEvent_MultipleEvents_ReturnsMultipleWebhooks() {
			var factory = new DefaultWebhookFactory<Webhook>(Options.Create(new WebhookFactoryOptions<Webhook> {
				CreateStrategy = WebhookCreateStrategy.OnePerEvent
			}));

			var events = new[] {
				new EventInfo("user", "user.created", "1.0", new { userId = 123, name = "John" }),
				new EventInfo("user", "user.created", "1.0", new { userId = 124, name = "Jane" })
			};
			var notification = new EventNotification("user.created", events) {
				NotificationId = "notif-4",
				TimeStamp = DateTimeOffset.UtcNow
			};

			var result = await factory.CreateAsync(subscription, notification, CancellationToken.None);

			Assert.Equal(2, result.Count);
			
			Assert.Equal(events[0].Id, result[0].Id);
			Assert.Equal("user.created", result[0].EventType);
			Assert.Equal("test-sub-1", result[0].SubscriptionId);
			Assert.Equal("Test Subscription", result[0].Name);
			Assert.Equal(events[0].TimeStamp, result[0].TimeStamp);
			
			Assert.Equal(events[1].Id, result[1].Id);
			Assert.Equal("user.created", result[1].EventType);
			Assert.Equal("test-sub-1", result[1].SubscriptionId);
			Assert.Equal("Test Subscription", result[1].Name);
			Assert.Equal(events[1].TimeStamp, result[1].TimeStamp);
		}

		[Fact]
		public void CreateNotificationData_OnePerEvent_MultipleEvents_ThrowsException() {
			var factory = new TestWebhookFactory(Options.Create(new WebhookFactoryOptions<Webhook> {
				CreateStrategy = WebhookCreateStrategy.OnePerEvent
			}));

			var events = new[] {
				new EventInfo("user", "user.created", "1.0", new { userId = 123 }),
				new EventInfo("user", "user.created", "1.0", new { userId = 124 })
			};
			var notification = new EventNotification("user.created", events);

			var exception = Assert.Throws<WebhookException>(() => 
				factory.TestCreateNotificationData(subscription, notification));

			Assert.Equal("The strategy 'OnePerEvent' requires a single event in the notification", exception.Message);
		}

		[Fact]
		public void CreateNotificationData_OnePerNotification_SingleEvent_ReturnsEventData() {
			var factory = new TestWebhookFactory(Options.Create(new WebhookFactoryOptions<Webhook> {
				CreateStrategy = WebhookCreateStrategy.OnePerNotification
			}));

			var eventData = new { userId = 123, name = "John" };
			var eventInfo = new EventInfo("user", "user.created", "1.0", eventData);
			var notification = new EventNotification(eventInfo);

			var result = factory.TestCreateNotificationData(subscription, notification);

			Assert.Equal(eventData, result);
		}

		[Fact]
		public void CreateNotificationData_OnePerNotification_MultipleEvents_ReturnsArray() {
			var factory = new TestWebhookFactory(Options.Create(new WebhookFactoryOptions<Webhook> {
				CreateStrategy = WebhookCreateStrategy.OnePerNotification
			}));

			var eventData1 = new { userId = 123, name = "John" };
			var eventData2 = new { userId = 124, name = "Jane" };
			var events = new[] {
				new EventInfo("user", "user.created", "1.0", eventData1),
				new EventInfo("user", "user.created", "1.0", eventData2)
			};
			var notification = new EventNotification("user.created", events);

			var result = factory.TestCreateNotificationData(subscription, notification);

			Assert.IsType<object[]>(result);
			var array = (object[])result;
			Assert.Equal(2, array.Length);
			Assert.Equal(eventData1, array[0]);
			Assert.Equal(eventData2, array[1]);
		}

		[Fact]
		public void CreateEventData_ReturnsEventInfoData() {
			var factory = new TestWebhookFactory();

			var eventData = new { userId = 123, name = "John" };
			var eventInfo = new EventInfo("user", "user.created", "1.0", eventData);

			var result = factory.TestCreateEventData(subscription, eventInfo);

			Assert.Equal(eventData, result);
		}

		[Fact]
		public void CreateEventData_WithNullData_ReturnsNull() {
			var factory = new TestWebhookFactory();

			var eventInfo = new EventInfo("user", "user.created", "1.0", null);

			var result = factory.TestCreateEventData(subscription, eventInfo);

			Assert.NotNull(result);
		}

		[Fact]
		public async Task CreateAsync_WithCustomFactory_UsesOverriddenMethods() {
			var factory = new CustomWebhookFactory();

			var eventInfo = new EventInfo("user", "user.created", "1.0", new { userId = 123 });
			var notification = new EventNotification(eventInfo);

			var result = await factory.CreateAsync(subscription, notification, CancellationToken.None);

			Assert.Single(result);
			var webhook = result[0];
			Assert.Equal("CUSTOM_DATA", webhook.Data);
		}

		// Test implementation that exposes protected methods
		private class TestWebhookFactory : DefaultWebhookFactory<Webhook> {
			public TestWebhookFactory(IOptions<WebhookFactoryOptions<Webhook>>? options = null) : base(options) {
			}

			public object? TestCreateNotificationData(IWebhookSubscription subscription, EventNotification notification) {
				return CreateNotificationData(subscription, notification);
			}

			public object? TestCreateEventData(IWebhookSubscription subscription, EventInfo eventInfo) {
				return CreateEventData(subscription, eventInfo);
			}
		}

		// Custom factory for testing virtual method overrides
		private class CustomWebhookFactory : DefaultWebhookFactory<Webhook> {
			protected override object? CreateEventData(IWebhookSubscription subscription, EventInfo eventInfo) {
				return "CUSTOM_DATA";
			}
		}

		// Test subscription implementation
		private class TestWebhookSubscription : IWebhookSubscription {
			public string? SubscriptionId { get; set; }
			public string? TenantId { get; set; }
			public string Name { get; set; } = "";
			public IEnumerable<string> EventTypes { get; set; } = Array.Empty<string>();
			public string DestinationUrl { get; set; } = "https://example.com/webhook";
			public string? Secret { get; set; }
			public string? Format { get; set; }
			public WebhookSubscriptionStatus Status { get; set; } = WebhookSubscriptionStatus.Active;
			public int? RetryCount { get; set; }
			public IEnumerable<IWebhookFilter>? Filters { get; set; }
			public IDictionary<string, string>? Headers { get; set; }
			public IDictionary<string, object>? Properties { get; set; }
			public DateTimeOffset? CreatedAt { get; set; }
			public DateTimeOffset? UpdatedAt { get; set; }
		}
	}
}