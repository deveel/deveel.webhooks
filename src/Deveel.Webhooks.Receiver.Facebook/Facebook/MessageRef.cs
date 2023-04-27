using System.Text.Json.Serialization;

namespace Deveel.Facebook {
	public readonly struct MessageRef {
		[JsonConstructor]
		public MessageRef(string id) {
			if (string.IsNullOrWhiteSpace(id)) 
				throw new ArgumentException($"'{nameof(id)}' cannot be null or whitespace.", nameof(id));

			Id = id;
		}

		[JsonPropertyName("mid")]
		public string Id { get; }

		public static implicit operator MessageRef(string id) {
			return new MessageRef(id);
		}
	}
}
