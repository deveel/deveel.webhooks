using System.Text.Json.Serialization;

namespace Deveel.Facebook {
	public sealed class OrderSummary {
		public OrderSummary(double totalCost) {
			TotalCost = totalCost;
		}

		[JsonPropertyName("total_cost")]
		public double TotalCost { get; }

		[JsonPropertyName("shipping_cost")]
		public double? ShippingCost { get; set; }

		[JsonPropertyName("total_tax")]
		public double? TotalTax { get; set; }

		[JsonPropertyName("subtotal")]
		public double? Subtotal { get; set; }
	}
}
