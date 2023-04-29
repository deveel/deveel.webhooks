using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Deveel.Webhooks.Json {
	class Base64StringConverter : JsonConverter<string> {
		public override string? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options) {
			if (reader.TokenType != JsonTokenType.String)
				throw new JsonException("Invalid token type found");

			var value = reader.GetString();

			if (String.IsNullOrWhiteSpace(value))
				return null;

			try {
				var bytes = Convert.FromBase64String(value);
				return Encoding.UTF8.GetString(bytes);
			} catch (FormatException) {
				return value;
			} catch (Exception) { 
				throw new JsonException("Invalid base64 string found");
			}
		}

		public override void Write(Utf8JsonWriter writer, string value, JsonSerializerOptions options) {
			var base64 = Convert.ToBase64String(Encoding.UTF8.GetBytes(value));
			writer.WriteStringValue(base64);
		}
	}
}
