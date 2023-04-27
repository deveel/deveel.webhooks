using System.Text.Json.Serialization;

namespace Deveel.Facebook {
    public readonly struct MessagingPart {
        [JsonConstructor]
        public MessagingPart(string id) {
            Id = id;
        }

        [JsonPropertyName("id")]
        public string Id { get; }

		public static implicit operator string(MessagingPart part) => part.Id;

		public static implicit operator MessagingPart(string id) => new MessagingPart(id);
    }
}
