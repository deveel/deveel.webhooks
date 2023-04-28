using System.Text.Json;
using System.Text.Json.Serialization;

namespace Deveel.Json {
	/// <summary>
	/// Converts Unix epoch milliseconds to and from <see cref="DateTimeOffset"/>.
	/// </summary>
    public sealed class UnixTimeMillisConverter : JsonConverter<DateTimeOffset> {
		/// <inheritdoc/>
        public override DateTimeOffset Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options) {
            if (!reader.TryGetInt64(out var value))
                throw new JsonException("Invalid input for a unix timestamp");

            return DateTimeOffset.FromUnixTimeMilliseconds(value);
        }
		
		/// <inheritdoc/>
        public override void Write(Utf8JsonWriter writer, DateTimeOffset value, JsonSerializerOptions options) {
            writer.WriteNumberValue(value.ToUnixTimeMilliseconds());
        }
    }
}
