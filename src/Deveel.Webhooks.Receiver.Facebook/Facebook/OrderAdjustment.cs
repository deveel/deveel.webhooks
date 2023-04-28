using System.Text.Json.Serialization;

namespace Deveel.Facebook {
	public sealed class OrderAdjustment {
		[JsonConstructor]
		public OrderAdjustment(string name, double amount) {
			if (string.IsNullOrWhiteSpace(name)) 
				throw new ArgumentException($"'{nameof(name)}' cannot be null or whitespace.", nameof(name));

			Name = name;
			Amount = amount;
		}

		[JsonPropertyName("name")]
		public string Name { get; }

		[JsonPropertyName("amount")]
		public double Amount { get; }
	}
}
