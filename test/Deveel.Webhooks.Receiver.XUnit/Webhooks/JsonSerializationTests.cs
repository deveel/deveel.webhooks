// Copyright 2022-2023 Deveel
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

using System.Text;
using System.Text.Json.Serialization;

using Xunit;

namespace Deveel.Webhooks {
	public static class JsonSerializationTests {
		private static async Task<TWebhook?> ParseWebhook<TWebhook>(string json) where TWebhook : class {
			var parser = new SystemTextWebhookJsonParser<TWebhook>();
			return await parser.ParseWebhookAsync(json);
		}

		private static async Task<IList<TWebhook>> ParseWebhookList<TWebhook>(string json) where TWebhook : class {
			var parser = new SystemTextWebhookJsonParser<TWebhook>();
			return await parser.ParseWebhookArrayAsync(json);
		}

		[Fact]
		public static async Task ParseWebhookAsync() {
			var json = "{\"event\":\"test\",\"name\":\"my_event\"}";

			var webhook = await ParseWebhook<TestWebhook>(json);

			Assert.NotNull(webhook);
			Assert.Equal("test", webhook.EventType);
			Assert.Equal("my_event", webhook.Name);
		}

		[Fact]
		public static async Task ParseWebhookListAsync() {
			var json = "[{\"event\":\"test\",\"name\":\"my_event\"},{\"event\":\"test2\",\"name\":\"my_event2\"}]";
			var webhooks = await ParseWebhookList<TestWebhook>(json);
			Assert.NotNull(webhooks);
			Assert.Equal(2, webhooks.Count);
			Assert.Equal("test", webhooks[0].EventType);
			Assert.Equal("my_event", webhooks[0].Name);
			Assert.Equal("test2", webhooks[1].EventType);
			Assert.Equal("my_event2", webhooks[1].Name);
		}

		class TestWebhook {
			[JsonPropertyName("event")]
			public string? EventType { get; set; }

			[JsonPropertyName("name")]
			public string? Name { get; set; }
		}
	}
}
