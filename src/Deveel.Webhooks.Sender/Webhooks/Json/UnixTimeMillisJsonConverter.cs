// Copyright 2022-2023 Deveel
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

using System;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Deveel.Webhooks.Json {
	/// <summary>
	/// A JSON converter that converts a <see cref="DateTimeOffset"/> to a Unix timestamp
	/// </summary>
	public sealed class UnixTimeMillisJsonConverter : JsonConverter<DateTimeOffset> {
		/// <inheritdoc/>
		public override DateTimeOffset Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options) {
			if (!reader.TryGetInt64(out var value))
				throw new JsonException("The timestamp is invalid: it must be a long");

			try {
				return DateTimeOffset.FromUnixTimeMilliseconds(value);
			} catch (Exception ex) {
				throw new JsonException($"Could not convert the value {value} to a proper date-time", ex);
			}
		}

		/// <inheritdoc/>
		public override void Write(Utf8JsonWriter writer, DateTimeOffset value, JsonSerializerOptions options) {
			writer.WriteNumberValue(value.ToUnixTimeMilliseconds());
		}
	}
}
