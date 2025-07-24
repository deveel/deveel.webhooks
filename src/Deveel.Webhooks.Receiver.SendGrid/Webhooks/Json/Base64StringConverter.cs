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
