using System;
using System.Text.Json.Serialization;

namespace Deveel.Facebook {
	public sealed class MediaElement {
		[JsonConstructor]
		public MediaElement(MediaElementType type) {
			Type = type;
		}

		[JsonPropertyName("media_type")]
		public MediaElementType Type { get; }

		[JsonPropertyName("attachment_id")]
		public string? AttachmentId { get; set; }

		[JsonPropertyName("url")]
		public string? Url { get; set; }

		[JsonPropertyName("buttons")]
		public IList<Button>? Buttons { get; set; }
	}
}
