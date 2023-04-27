using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Deveel.Facebook {
	public sealed class ProductElement {
		[JsonPropertyName("id")]
		public string? Id { get; set; }

		[JsonPropertyName("title")]
		public string? Title { get; set; }

		[JsonPropertyName("subtitle")]
		public string? Subtitle { get; set; }

		[JsonPropertyName("image_url")]
		public string? ImageUrl { get; set; }

		[JsonPropertyName("retailer_id")]
		public string? RetailerId { get; set; }
	}
}
