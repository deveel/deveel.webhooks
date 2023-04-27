using System.Text.Json.Serialization;

namespace Deveel.Facebook {
	public sealed class OrderElement {
		public OrderElement(string title, double price) {
			if (string.IsNullOrWhiteSpace(title)) 
				throw new ArgumentException($"'{nameof(title)}' cannot be null or whitespace.", nameof(title));

			if (price < 0) 
				throw new ArgumentOutOfRangeException(nameof(price));

			Title = title;
			Price = price;
		}

		[JsonPropertyName("title")]
		public string Title { get; }

		[JsonPropertyName("price")]
		public double Price { get; }

		[JsonPropertyName("subtitle")]
		public string? Subtitle { get; set; }

		[JsonPropertyName("currency")]
		public string? Currency { get; set; }

		[JsonPropertyName("image_url")]
		public string? ImageUrl { get; set; }

		[JsonPropertyName("quantity")]
		public double? Quantity { get; set; }
	}
}
