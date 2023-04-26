using System.Text.Json.Serialization;

namespace Deveel.Webhooks.Models {
	public class User {
		[JsonPropertyName("id")]
		public string Id { get; set; }

		[JsonPropertyName("name")]
		public string Name { get; set; }

		[JsonPropertyName("email")]
		public string Email { get; set; }

		[JsonPropertyName("role")]
		public string Role { get; set; }
	}
}
