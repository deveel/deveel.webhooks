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

using System.Text.Json;
using System.Text.Json.Serialization;

namespace Deveel.Webhooks {
	public class LinqEvaluatorTests {
		private readonly IWebhookFilterEvaluator<TestWebhook> evaluator;

		public LinqEvaluatorTests() {
			evaluator = LinqWebhookFilterEvaluator<TestWebhook>.Default;
		}

		[Fact]
		public async Task EvaluateWildcard() {
			var webhook = new TestWebhook {
				Id = "123",
				EventName = "test",
				Data = JsonSerializer.Deserialize<IDictionary<string, JsonElement>>("{\"foo\": \"bar\"}")
			};

			var result = await evaluator.MatchesAsync(WebhookSubscriptionFilter.Create("linq", "*"), webhook);

			Assert.True(result);
		}

		[Fact]
		public async Task EvaluateSimpleFilterOnExtendedData() {
			var webhook = new TestWebhook {
				Id = "123",
				EventName = "test",
				Data = JsonSerializer.Deserialize<IDictionary<string, JsonElement>>("{\"foo\": \"bar\"}")
			};

			var result = await evaluator.MatchesAsync(WebhookSubscriptionFilter.Create("linq", "foo == \"bar\""), webhook);

			Assert.True(result);

			result = await evaluator.MatchesAsync(WebhookSubscriptionFilter.Create("linq", "foo == \"baz\""), webhook);

			Assert.False(result);
		}

		[Fact]
		public async Task EvaluateSimpleFilterOnExtendedDataWithWildcard() {
			var webhook = new TestWebhook {
				Id = "123",
				EventName = "test",
				Data = JsonSerializer.Deserialize<IDictionary<string, JsonElement>>("{\"foo\": \"bar\"}")
			};

			var result = await evaluator.MatchesAsync(WebhookSubscriptionFilter.Create("linq", "foo == \"bar\"", "*"), webhook);
			Assert.True(result);
		}

		[Fact]
		public async Task EvaluateOnEmptyWebhook() {
			var webhook = new TestWebhook();

			var result = await evaluator.MatchesAsync(WebhookSubscriptionFilter.Create("linq", "event_name == \"data.created\""), webhook);

			Assert.False(result);
		}

		[Fact]
		public async Task EvaluateComplexWebhook() {
			var webhook = new TestWebhookWithComplexData {
				Id = "123",
				WebhookName = "test",
				Data = new EventData {
					Count = 10
				},
				EventType = new EventType {
					StreamType = "test",
					TypeName = "test",
					Version = "1.0"
				}
			};

			var evaluator = LinqWebhookFilterEvaluator<TestWebhookWithComplexData>.Default;
			var filter = WebhookSubscriptionFilter.Create("linq", "data.count == 10 && event_type.type == \"test\"");
			var result = await evaluator.MatchesAsync(filter, webhook, default);

			Assert.True(result);
		}

		[Fact]
		public async Task EvaluateNotLinq() {
			await Assert.ThrowsAsync<ArgumentException>(() => evaluator.MatchesAsync(WebhookSubscriptionFilter.Create("json", "event_name == \"data.created\""), new TestWebhook()));
		}

		class TestWebhook {
			[JsonPropertyName("id")]
			public string Id { get; set; }

			[JsonPropertyName("event_name")]
			public string EventName { get; set; }

			[JsonExtensionData]
			public IDictionary<string, JsonElement>? Data { get; set; }
		}

		class TestWebhookWithComplexData {
			[JsonPropertyName("id")]
			public string Id { get; set; }
			
			[JsonPropertyName("name")]
			public string WebhookName { get; set; }
			
			[JsonPropertyName(("data"))]
			public EventData Data { get; set; }
			
			[JsonPropertyName("event_type")]
			public EventType EventType { get; set; }
		}

		class EventData {
			[JsonPropertyName("count")] 
			public int Count { get; set; }
		}

		class EventType {
			[JsonPropertyName("stream_type")]
			public string StreamType { get; set; }
			
			[JsonPropertyName("type")]
			public string TypeName { get; set; }
			
			[JsonPropertyName("version")]
			public string Version { get; set; }
		}
	}
}
