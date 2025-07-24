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

using System.Text.Json;
using System.Text.Json.Nodes;
using System.Text.Json.Serialization;

using Deveel.Webhooks.Json;

namespace Deveel.Webhooks {
	public static class SystemTextJsonSerializationTests {
		[Fact]
		public static async Task SerializeToString() {
			var serializer = new SystemTextWebhookJsonSerializer<TestWebhook>();

			var id = Guid.NewGuid().ToString("N");

			var webhook = new TestWebhook {
				Name = "Test Event",
				Type = "test",
				IgnoredTime = DateTime.Now,
				TimeStamp = DateTimeOffset.UtcNow,
				States = new[] {"pending", "active"},
				Id = id,
				Data = new Dictionary<string, JsonElement> {
					{ "foo", JsonDocument.Parse("\"bar\"").RootElement }
				}
			};

			var json = await serializer.SerializeToStringAsync(webhook);

			Assert.NotNull(json);
			Assert.NotEmpty(json);

			var obj = JsonObject.Parse(json);
			Assert.NotNull(obj);
			Assert.NotNull(obj["name"]);
			Assert.NotNull(obj["event_type"]);
			Assert.Null(obj["IgnoredTime"]);
			Assert.NotNull(obj["states"]);
			Assert.NotNull(obj["event_id"]);
			Assert.NotNull(obj["timestamp"]);
			Assert.NotNull(obj["foo"]);

			Assert.Equal(7, obj.AsObject().Count);

			Assert.Equal("Test Event", obj["name"]!.ToString());
			Assert.Equal("test", obj["event_type"]!.ToString());
			Assert.Equal(id, obj["event_id"]!.ToString());
			Assert.NotNull(obj["states"]);
			Assert.Equal(2, obj["states"]!.AsArray().Count);
			Assert.Equal("pending", obj["states"]![0]!.ToString());
		}

		[Fact]
		public static async Task SerializeInvalidObjectToString() {
			var serializer = new SystemTextWebhookJsonSerializer<InvalidTestWebhook>();

			var webhook = new InvalidTestWebhook {
				Name = "Test Event",
				Type = "test",
				DataType = typeof(string),
			};

			var error = await Assert.ThrowsAsync<WebhookSerializationException>(() => serializer.SerializeToStringAsync(webhook));

			Assert.NotNull(error);
		}

		[Fact]
		public static async Task SerializeToAnonymousType() {
			var serializer = new SystemTextWebhookJsonSerializer<TestWebhook>();

			var id = Guid.NewGuid().ToString();
			var timestamp = DateTimeOffset.UtcNow;

			var webhook = new TestWebhook {
				Name = "Test Event",
				Type = "test",
				IgnoredTime = DateTime.Now,
				TimeStamp = timestamp,
				States = new[] { "pending", "active" },
				Id = id,
				Count = 45,
				Data = new Dictionary<string, JsonElement> {
					{ "foo", JsonDocument.Parse("\"bar\"").RootElement }
				}
			};

			var obj = await serializer.SerializeToObjectAsync(webhook);

			Assert.NotNull(obj);
			Assert.Equal("Deveel.Webhooks.Types", obj.GetType().Namespace);

			Assert.NotNull(obj.GetType().GetProperty("event_type"));
			Assert.NotNull(obj.GetType().GetProperty("name"));
			Assert.Null(obj.GetType().GetProperty("IgnoredTime"));
			Assert.NotNull(obj.GetType().GetProperty("event_id"));
			Assert.NotNull(obj.GetType().GetProperty("timestamp"));
			Assert.NotNull(obj.GetType().GetProperty("states"));
			Assert.NotNull(obj.GetType().GetProperty("foo"));
			Assert.NotNull(obj.GetType().GetProperty("count"));

			Assert.Equal("Test Event", obj.GetType().GetProperty("name")!.GetValue(obj));
			Assert.Equal("test", obj.GetType().GetProperty("event_type")!.GetValue(obj));
			var objId = Assert.IsType<Guid>(obj.GetType().GetProperty("event_id")!.GetValue(obj));
			Assert.Equal(Guid.Parse(id), objId);
			Assert.Equal(45, obj.GetType().GetProperty("count")!.GetValue(obj));
			Assert.Equal(timestamp.ToUnixTimeMilliseconds(), obj.GetType().GetProperty("timestamp")!.GetValue(obj));
		}

#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.

        class TestWebhook {
			[JsonPropertyName("name")]
			public string Name { get; set; }

			[JsonPropertyName("event_type")]
            public string Type { get; set; }

            [JsonIgnore]
			public DateTime IgnoredTime { get; set; }

			[JsonPropertyName("states")]
			public string[] States { get; set; }

			[JsonPropertyName("event_id")]
			public string Id { get; set; }

			[JsonPropertyName("timestamp")]
			[JsonConverter(typeof(UnixTimeMillisJsonConverter))]
			public DateTimeOffset TimeStamp { get; set; }

			[JsonPropertyName("count")]
			public int? Count { get; set; }

			[JsonExtensionData]
			public IDictionary<string, JsonElement> Data { get; set; }
		}

		class InvalidTestWebhook {
			[JsonPropertyName("name")]
			public string Name { get; set; }

			[JsonPropertyName("event_type")]
			public string Type { get; set; }


			[JsonPropertyName("data_type")]
			public Type DataType { get; set; }
		}
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
    }
}
