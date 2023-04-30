using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace Deveel.Webhooks {
	public static class NewtonsoftJsonParserTests {
		private static readonly NewtonsoftWebhookJsonParser<TestWebhook> Parser;

		static NewtonsoftJsonParserTests() {
			Parser = new NewtonsoftWebhookJsonParser<TestWebhook>();
		}

		[Fact]
		public static async Task ParseTestWebhook() {
			var time = DateTimeOffset.UtcNow.ToUnixTimeSeconds();

			var json = "{\"event_id\": \"abc1234567890\", \"event_name\":\"test\",\"event_time\":"+ time +"}";
			using var stream = new MemoryStream(Encoding.UTF8.GetBytes(json));

			var result = await Parser.ParseWebhookAsync(stream);

			Assert.NotNull(result);
			Assert.Equal("abc1234567890", result!.Id);
			Assert.Equal("test", result.EventName);
			Assert.Equal(time, result.EventTime.ToUnixTimeSeconds());
		}

		[Fact]
		public static async Task ParseTestWebhookArray() {
			var time = DateTimeOffset.UtcNow.ToUnixTimeSeconds();
			var json = "[{\"event_id\": \"abc1234567890\", \"event_name\":\"test\",\"event_time\":"+ time +"}]";

			using var stream = new MemoryStream(Encoding.UTF8.GetBytes(json));
			var result = await Parser.ParseWebhookArrayAsync(stream);

			Assert.NotNull(result);
			Assert.Single(result);
			Assert.Equal("abc1234567890", result.First().Id);
			Assert.Equal("test", result.First().EventName);
			Assert.Equal(time, result.First().EventTime.ToUnixTimeSeconds());
		}

		class TestWebhook {
			[JsonProperty("event_id")]
			public string Id { get; set; }

			[JsonProperty("event_name")]
			public string? EventName { get; set; }

			[JsonProperty("event_time")]
			[JsonConverter(typeof(UnixDateTimeConverter))]
			public DateTimeOffset EventTime { get; set; }
		}
	}
}
