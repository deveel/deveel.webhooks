using System;
using System.Text.Json.Serialization;

namespace Deveel.Facebook {
	public sealed class TemplateElement {
		[JsonConstructor]
		public TemplateElement(string title) {
			if (string.IsNullOrWhiteSpace(title)) 
				throw new ArgumentException($"'{nameof(title)}' cannot be null or whitespace.", nameof(title));

			Title = title;
		}

		[JsonPropertyName("title")]
		public string Title { get; }

		[JsonPropertyName("subtitle")]
		public string? Subtitle { get; set; }

		[JsonPropertyName("image_url")]
		public string? ImageUrl { get; set; }

		[JsonPropertyName("default_action")]
		public WebUrlButton? DefaultAction { get; set; }

		[JsonPropertyName("buttons")]
		public IList<Button>? Buttons { get; set; }
	}
}
