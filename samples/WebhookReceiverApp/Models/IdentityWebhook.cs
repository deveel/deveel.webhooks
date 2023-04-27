using System.Text.Json.Serialization;

namespace Deveel.Webhooks.Models {
	public class IdentityWebhook {
		[JsonPropertyName("event_name")]
		public string EventName { get; set; }

		[JsonPropertyName("event_id")]
		public string EventId { get; set; }

		[JsonPropertyName("timestamp")]
		public DateTimeOffset TimeStamp { get; set; }

		[JsonPropertyName("user")]
		public User? User { get; set; }
	}
}
