using System.Text.Json.Serialization;

namespace Deveel.Webhooks.Models {
	public class IdentityWebhook {
		[JsonPropertyName("event_type")]
		public string EventType { get; set; }

		[JsonPropertyName("event_id")]
		public string EventId { get; set; }

		[JsonPropertyName("timestamp")]
		[JsonConverter(typeof(UnixTimeMillisJsonConverter))]
		public DateTimeOffset TimeStamp { get; set; }

		[JsonPropertyName("user")]
		public User? User { get; set; }
	}
}
