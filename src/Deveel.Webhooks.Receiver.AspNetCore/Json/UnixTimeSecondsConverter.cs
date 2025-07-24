// Copyright 2022-2025 Antonello Provenzano
// 
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
// 
//     http://www.apache.org/licenses/LICENSE-2.0
// 
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.

using System.Text.Json;
using System.Text.Json.Serialization;

namespace Deveel.Json {
	/// <summary>
	/// A converter for the Unix time seconds format.
	/// </summary>
	public sealed class UnixTimeSecondsConverter : JsonConverter<DateTimeOffset> {
		/// <summary>
		/// Converts a Unix time seconds value to a <see cref="DateTimeOffset"/> value.
		/// </summary>
		/// <param name="reader">
		/// The JSON reader to read from.
		/// </param>
		/// <param name="typeToConvert">
		/// The type to convert to.
		/// </param>
		/// <param name="options">
		/// An optional set of serializer options.
		/// </param>
		/// <returns>
		/// Returns a <see cref="DateTimeOffset"/> value that is equivalent
		/// to the Unix time seconds value.
		/// </returns>
		/// <exception cref="JsonException"></exception>
		public override DateTimeOffset Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options) {
			if (typeToConvert != typeof(DateTimeOffset))
				throw new JsonException($"Cannot convert to {typeToConvert}");

			if (reader.TryGetInt64(out var l))
				return DateTimeOffset.FromUnixTimeSeconds(l);
			if (reader.TryGetInt32(out var i))
				return DateTimeOffset.FromUnixTimeSeconds(i);

			throw new JsonException("Expected an integer value for the Unix time seconds");
		}

		/// <summary>
		/// Writes a <see cref="DateTimeOffset"/> value as a Unix time seconds value.
		/// </summary>
		/// <param name="writer">
		/// The JSON writer to write to.
		/// </param>
		/// <param name="value">
		/// The date time offset value to write as a Unix time seconds value.
		/// </param>
		/// <param name="options">
		/// An optional set of serializer options.
		/// </param>
		public override void Write(Utf8JsonWriter writer, DateTimeOffset value, JsonSerializerOptions options) {
			writer.WriteNumberValue(value.ToUnixTimeSeconds());
		}
	}
}
