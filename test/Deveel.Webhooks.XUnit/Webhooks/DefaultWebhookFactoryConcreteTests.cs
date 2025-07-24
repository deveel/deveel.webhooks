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

using Microsoft.Extensions.Options;
using Xunit;

namespace Deveel.Webhooks {
	public class DefaultWebhookFactoryConcreteTests {
		[Fact]
		public async Task CreateAsync_WorksCorrectly() {
			var options = Options.Create(new WebhookFactoryOptions<Webhook> {
				CreateStrategy = WebhookCreateStrategy.OnePerNotification
			});
			var factory = new DefaultWebhookFactory(options);

			var subscription = new TestWebhookSubscription {
				SubscriptionId = "test-sub",
				Name = "Test Sub"
			};

			var eventInfo = new EventInfo("user", "user.created", "1.0", new { userId = 123 });
			var notification = new EventNotification(eventInfo) {
				NotificationId = "notif-1",
				TimeStamp = DateTimeOffset.UtcNow
			};

			var result = await factory.CreateAsync(subscription, notification, CancellationToken.None);

			Assert.Single(result);
			Assert.Equal("notif-1", result[0].Id);
			Assert.Equal("user.created", result[0].EventType);
		}

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