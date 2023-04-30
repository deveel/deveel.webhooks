namespace Deveel.Webhooks.Models {
	public class UserCreatedEvent : IEventInfo {
		public string TenantId { get; set; }

		public string UserId { get; set; }

		string IEventInfo.Subject => "user";

		string IEventInfo.EventType => "created";

		string IEventInfo.Id { get; }

		DateTimeOffset IEventInfo.TimeStamp => DateTimeOffset.UtcNow;

		string? IEventInfo.DataVersion => "1.0";

		object? IEventInfo.Data => new {
			tenant = TenantId,
			user = UserId
		};
	}
}
