using System.Text.Json.Serialization;

using Deveel.Json;

namespace Deveel.Facebook {
    public sealed class MessageRead {
        [JsonConstructor]
        public MessageRead(DateTimeOffset watermark) {
            Watermark = watermark;
        }

        [JsonPropertyName("watermark")]
		[JsonConverter(typeof(UnixTimeMillisConverter))]
        public DateTimeOffset Watermark { get; }
    }
}
