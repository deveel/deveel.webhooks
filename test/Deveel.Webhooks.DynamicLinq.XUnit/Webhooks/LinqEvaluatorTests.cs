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

using System;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Deveel.Webhooks {
	public class LinqEvaluatorTests {
		private readonly IWebhookFilterEvaluator<TestWebhook> evaluator;

		public LinqEvaluatorTests() {
			evaluator = new LinqWebhookFilterEvaluator<TestWebhook>(new SystemTextWebhookJsonSerializer<TestWebhook>());
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
	}
}
