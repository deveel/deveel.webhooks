// Copyright 2022-2024 Antonello Provenzano
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

using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Deveel.Webhooks.Facebook {
    /// <summary>
    /// A utility class that can be used to serialize and deserialize
    /// Facebook entities to and from JSON
    /// </summary>
    public static class FacebookJsonSerializer {
        static FacebookJsonSerializer() {
            var serializerOptions = new JsonSerializerOptions {
                DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull,
                PropertyNameCaseInsensitive = true,
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase
            };

            serializerOptions.Converters.Add(new JsonStringEnumMemberConverter(JsonNamingPolicy.CamelCase));

            Options = serializerOptions;
        }

        /// <summary>
        /// Gets the serialization options
        /// </summary>
        public static JsonSerializerOptions Options { get; }

        /// <summary>
        /// Deserializes a given UTF-8 stream to a Facebook Messenger webhook
        /// </summary>
        /// <param name="utf8Stream">The UTF-8 to be read for deserialization</param>
        /// <returns>
        /// Returns an instance of <see cref="FacebookWebhook"/> that is the result
        /// of the deserialization.
        /// </returns>
        public static FacebookWebhook? DeserializeWebhook(Stream utf8Stream)
            => JsonSerializer.Deserialize<FacebookWebhook>(utf8Stream, Options);

        /// <summary>
        /// Deserializes a given UTF-8 stream to a Facebook webhook
        /// </summary>
        /// <param name="utf8Stream">The UTF-8 to be read for deserialization</param>
        /// <param name="cancellationToken"></param>
        /// <returns>
        /// Returns an instance of <see cref="FacebookWebhook"/> that is the result
        /// of the deserialization.
        /// </returns>
        public static async Task<FacebookWebhook?> DeserializeWebhookAsync(Stream utf8Stream, CancellationToken cancellationToken = default)
            => await JsonSerializer.DeserializeAsync<FacebookWebhook>(utf8Stream, Options, cancellationToken);

        /// <summary>
        /// Deserializes a given JSON-formatted string to a Facebook webhook
        /// </summary>
        /// <param name="json">The JSON-formatted string to read for deserialization</param>
        /// <returns>
        /// Returns an instance of <see cref="FacebookWebhook"/> that is the result
        /// of the deserialization.
        /// </returns>
        public static FacebookWebhook? DeserializeWebhook(string json) {
            return JsonSerializer.Deserialize<FacebookWebhook>(json, Options);
        }
    }
}