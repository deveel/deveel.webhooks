using System.Text.Json.Serialization;

namespace Deveel.Facebook {
	public sealed class PostbackButton : Button {
		public PostbackButton() : base(ButtonType.Postback) {
		}

		[JsonPropertyName("title")]
		public string? Title { get; set; }

		[JsonPropertyName("payload")]
		public object? Payload { get; set; }
	}
}
