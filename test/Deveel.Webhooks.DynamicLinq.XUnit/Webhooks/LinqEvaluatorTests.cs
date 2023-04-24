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
