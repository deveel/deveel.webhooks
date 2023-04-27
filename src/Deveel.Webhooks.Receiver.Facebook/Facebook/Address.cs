using System.Text.Json.Serialization;

namespace Deveel.Facebook {
	/// <summary>
	/// Describes a postal address.
	/// </summary>
	public sealed class Address {
		/// <summary>
		/// The first line of the address.
		/// </summary>
		[JsonPropertyName("street_1")]
		public string StreetLine1 { get; set; }

		/// <summary>
		/// An optional second line of the address.
		/// </summary>
		[JsonPropertyName("street_2")]
		public string? StreetLine2 { get; set; }

		/// <summary>
		/// The postal city of the address.
		/// </summary>
		[JsonPropertyName("city")]
		public string City { get; set; }

		/// <summary>
		/// The postal code of the address.
		/// </summary>
		[JsonPropertyName("postal_code")]
		public string PostalCode { get; set; }

        /// <summary>
        /// The state abbreviation for U.S. addresses, or the 
		/// region/province for non-U.S. addresses.
        /// </summary>
        [JsonPropertyName("state")]
		public string State { get; set; }

		/// <summary>
		/// The two-letter ISO 3166-1 country code of the address.
		/// </summary>
		[JsonPropertyName("country")]
		public string Country { get; set; }
	}
}
