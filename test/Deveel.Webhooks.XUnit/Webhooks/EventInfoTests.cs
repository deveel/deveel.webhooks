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

using System.Collections.Generic;
using System.Text.Json;
using Xunit;

namespace Deveel.Webhooks {
	public static class EventInfoTests {
		[Fact]
		public static void CreateEventInfo_WithAnonymousData() {
			var userCreated = new UserCreatedModel {
				TenantId = "test",
				UserId = "user1"
			};

			var eventInfo = userCreated.AsEventInfo();

			Assert.Equal("user", eventInfo.Subject);
			Assert.Equal("created", eventInfo.EventType);
			Assert.Equal("1.0", eventInfo.DataVersion);
			Assert.NotNull(eventInfo.Id);
			Assert.NotNull(eventInfo.Data);
			Assert.Equal("test", eventInfo.GetValue<string>("tenant"));
			Assert.Equal("user1", eventInfo.GetValue<string>("user"));
			Assert.Null(eventInfo.GetValue<string?>("unknown"));
		}

		[Fact]
		public static void CreateEventInfo_WithDictionaryData() {
			var userCreated = new UserDeletedModel {
				TenantId = "test",
				UserId = "user1"
			};

			var eventInfo = userCreated.AsEventInfo();
			Assert.NotNull(eventInfo.Id);
			Assert.NotNull(eventInfo.Data);
			Assert.Equal("user", eventInfo.Subject);
			Assert.Equal("deleted", eventInfo.EventType);
			Assert.Equal("1.0", eventInfo.DataVersion);
			Assert.Equal("test", eventInfo.GetValue<string>("tenant"));
			Assert.Equal("user1", eventInfo.GetValue<string>("user"));
			Assert.Null(eventInfo.GetValue<string?>("unknown"));
		}

		[Fact]
		public static void CreateEventInfo_WithNoData() {
			var ping = new PingEvent();

			var eventInfo = ping.AsEventInfo();

			Assert.NotNull(eventInfo.Id);
			Assert.NotNull(eventInfo.Data);
			Assert.Equal("system", eventInfo.Subject);
			Assert.Equal("ping", eventInfo.EventType);
			Assert.Equal("0.1", eventInfo.DataVersion);
			Assert.Null(eventInfo.GetValue<string?>("unknown"));
		}

		[Fact]
		public static void AsEventInfo_NullInstance_ThrowsArgumentNullException() {
			IEventInfo nullEventInfo = null!;
			Assert.Throws<ArgumentNullException>(() => nullEventInfo.AsEventInfo());
		}

		[Fact]
		public static void TryGetValue_NullEventInfo_ThrowsArgumentNullException() {
			Assert.Throws<ArgumentNullException>(() => EventInfoExtensions.TryGetValue<string>(null!, "any", out _));
		}

		[Fact]
		public static void TryGetValue_DataNull_ReturnsFalse() {
			var eventInfo = new PingEvent();

			var found = eventInfo.TryGetValue<string>("any", out var value);

			Assert.False(found);
			Assert.Null(value);
		}

		[Fact]
		public static void TryGetValue_DictionaryPathMissing_ReturnsFalse() {
			var eventInfo = new UserDeletedModel {
				TenantId = "test",
				UserId = "user1"
			};

			var found = eventInfo.TryGetValue<string>("missing", out var value);

			Assert.False(found);
			Assert.Null(value);
		}

		[Fact]
		public static void TryGetValue_JsonDictionary_ThrowsNotSupportedException() {
			var json = JsonSerializer.Deserialize<JsonElement>("{\"tenant\":\"test\"}");
			var eventInfo = new GenericEventInfo {
				Data = new Dictionary<string, JsonElement> {
					["root"] = json
				}
			};

			Assert.Throws<NotSupportedException>(() => eventInfo.TryGetValue<string>("root", out _));
		}

		[Fact]
		public static void TryGetValue_NestedPropertyPath_ReturnsTrueAndValue() {
			var eventInfo = new GenericEventInfo {
				Data = new {
					tenant = new {
						id = "tenant-1"
					}
				}
			};

			var found = eventInfo.TryGetValue<string>("tenant.id", out var value);

			Assert.True(found);
			Assert.Equal("tenant-1", value);
		}

		[Fact]
		public static void TryGetValue_FieldPath_ReturnsTrueAndValue() {
			var eventInfo = new GenericEventInfo {
				Data = new FieldContainer {
					payload = new FieldPayload { id = 42 }
				}
			};

			var found = eventInfo.TryGetValue<int>("payload.id", out var value);

			Assert.True(found);
			Assert.Equal(42, value);
		}

		[Fact]
		public static void TryGetValue_ConvertibleValue_ConvertsInvariantCulture() {
			var eventInfo = new GenericEventInfo {
				Data = new Dictionary<string, object> {
					["retryCount"] = 7
				}
			};

			var found = eventInfo.TryGetValue<string>("retryCount", out var value);

			Assert.True(found);
			Assert.Equal("7", value);
		}

		[Fact]
		public static void TryGetValue_NonConvertibleValue_Throws() {
			var eventInfo = new GenericEventInfo {
				Data = new Dictionary<string, object> {
					["createdAt"] = "not-a-date"
				}
			};

			Assert.ThrowsAny<Exception>(() => eventInfo.TryGetValue<DateTime>("createdAt", out _));
		}

		class UserCreatedModel : IEventInfo {
			public string UserId { get; set; }

			public string TenantId { get; set; }

			string IEventInfo.Subject => "user";

			string IEventInfo.EventType => "created";

			string IEventInfo.Id { get; } = Guid.NewGuid().ToString();

			DateTimeOffset IEventInfo.TimeStamp => DateTimeOffset.UtcNow;

			string? IEventInfo.DataVersion => "1.0";

			object? IEventInfo.Data => new {
				tenant = TenantId,
				user = UserId
			};
		}

		class UserDeletedModel : IEventInfo {
			public string UserId { get; set; }

			public string TenantId { get; set; }

			string IEventInfo.Subject => "user";

			string IEventInfo.EventType => "deleted";

			string IEventInfo.Id { get; } = Guid.NewGuid().ToString();

			DateTimeOffset IEventInfo.TimeStamp => DateTimeOffset.UtcNow;

			string? IEventInfo.DataVersion => "1.0";

			object? IEventInfo.Data => new Dictionary<string, object> {
				{ "tenant", TenantId },
				{ "user", UserId }
			};
		}

		class PingEvent : IEventInfo {
			string IEventInfo.Subject => "system";

			string IEventInfo.EventType => "ping";

			string IEventInfo.Id { get; } = Guid.NewGuid().ToString();

			DateTimeOffset IEventInfo.TimeStamp => DateTimeOffset.UtcNow;

			string? IEventInfo.DataVersion => "0.1";

			object? IEventInfo.Data => null;
		}

		private sealed class GenericEventInfo : IEventInfo {
			public string Subject => "generic";

			public string EventType => "test";

			public string Id { get; } = Guid.NewGuid().ToString();

			public DateTimeOffset TimeStamp => DateTimeOffset.UtcNow;

			public string? DataVersion => "1.0";

			public object? Data { get; set; }
		}

		private sealed class FieldContainer {
			public FieldPayload payload = null!;
		}

		private sealed class FieldPayload {
			public int id;
		}
	}
}
