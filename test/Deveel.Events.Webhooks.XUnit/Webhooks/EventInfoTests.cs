using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
	}
}
