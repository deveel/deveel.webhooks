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
    /// Represents a marker that indicates that 
    /// messages sent has been read.
    /// </summary>
    public sealed class MessageRead {
        /// <summary>
        /// Constructs the message read marker with
        /// the given <paramref name="watermark"/>.
        /// </summary>
        /// <param name="watermark">
        /// The timestamp that marks all messages as read
        /// </param>
        [JsonConstructor]
        public MessageRead(DateTimeOffset watermark) {
            Watermark = watermark;
        }

        /// <summary>
        /// Gets the timestamp that marks all messages
        /// sent before this time has been read by the
        /// recipient.
        /// </summary>
        [JsonPropertyName("watermark")]
        [JsonConverter(typeof(UnixTimeMillisConverter))]
        public DateTimeOffset Watermark { get; }
    }
}