using System.Text.Json.Serialization;

namespace Deveel.Facebook {
	public sealed class PageReference {
		[JsonConstructor]
		public PageReference(string id) {
			if (string.IsNullOrWhiteSpace(id)) 
				throw new ArgumentException($"'{nameof(id)}' cannot be null or whitespace.", nameof(id));

			Id = id;
		}

		[JsonPropertyName("id")]
		public string Id { get; }
	}
}
