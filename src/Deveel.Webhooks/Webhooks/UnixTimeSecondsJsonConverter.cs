using System;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Deveel.Webhooks {
	/// <summary>
	/// A JSON converter that converts a <see cref="DateTimeOffset"/> to a Unix timestamp
	/// </summary>
	public sealed class UnixTimeSecondsJsonConverter : JsonConverter<DateTimeOffset> {
		/// <inheritdoc/>
		public override DateTimeOffset Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options) {
			if (!reader.TryGetInt64(out var value))
				throw new JsonException("The timestamp is invalid: it must be a long");

			try {
				return DateTimeOffset.FromUnixTimeSeconds(value);
			} catch (Exception ex) {
				throw new JsonException($"Could not convert the value {value} to a proper date-time", ex);
			}
		}

		/// <inheritdoc/>
		public override void Write(Utf8JsonWriter writer, DateTimeOffset value, JsonSerializerOptions options) {
			writer.WriteNumberValue(value.ToUnixTimeSeconds());
		}
	}
}
