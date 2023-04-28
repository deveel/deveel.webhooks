using System.Text.Json.Serialization;

using Deveel.Json;

namespace Deveel.Facebook {
    public sealed class MessageDelivery {
        [JsonConstructor]
        public MessageDelivery(DateTimeOffset watermark) {
            Watermark = watermark;
        }

        [JsonPropertyName("mids")]
        public string[]? MessageIds { get; set; }

        [JsonPropertyName("watermark")]
        [JsonConverter(typeof(UnixTimeMillisConverter))]
        public DateTimeOffset Watermark { get; }
    }
}
