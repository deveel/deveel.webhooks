using System;
using System.Text.Json.Serialization;

namespace Deveel.Facebook {
	public sealed class TextQuickReply : QuickReply {
		public TextQuickReply(string title, string payload)
			: base(QuickReplyType.Text) {
			if (string.IsNullOrWhiteSpace(title)) 
				throw new ArgumentException($"'{nameof(title)}' cannot be null or whitespace.", nameof(title));
			if (string.IsNullOrWhiteSpace(payload)) 
				throw new ArgumentException($"'{nameof(payload)}' cannot be null or whitespace.", nameof(payload));

			Title = title;
			Payload = payload;
		}

		[JsonPropertyName("title")]
		public string Title { get; }

		[JsonPropertyName("payload")]
		public string Payload { get; }

		[JsonPropertyName("image_url")]
		public string? ImageUrl { get; set; }
	}
}
