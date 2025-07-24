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

using System.Text.Json.Serialization;

using Deveel.Json;

namespace Deveel.Webhooks.Facebook {
    /// <summary>
    /// Informs that messages sent from a page have been delivered.
    /// </summary>
    public sealed class MessageDelivery {
        /// <summary>
        /// Initializes the delivery with the given <paramref name="watermark"/>
        /// timestamp.
        /// </summary>
        /// <param name="watermark"></param>
        [JsonConstructor]
        public MessageDelivery(DateTimeOffset watermark) {
            Watermark = watermark;
        }

        /// <summary>
        /// Gets an optional array of message IDs that were delivered.
        /// </summary>
        [JsonPropertyName("mids")]
        public string[]? MessageIds { get; set; }

        /// <summary>
        /// Gets the timestamp that indicates that all messages
        /// before this watermark were delivered.
        /// </summary>
        [JsonPropertyName("watermark")]
        [JsonConverter(typeof(UnixTimeMillisConverter))]
        public DateTimeOffset Watermark { get; }
    }
}