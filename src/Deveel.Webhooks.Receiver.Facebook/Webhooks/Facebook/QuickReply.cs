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

using System.Text.Json.Serialization;

namespace Deveel.Webhooks.Facebook {
    /// <summary>
    /// Represents a quick reply to a message.
    /// </summary>
    public sealed class QuickReply {
        /// <summary>
        /// Initializes the quick reply with the given message.
        /// </summary>
        /// <param name="payload">
		/// The message payload of the quick reply.
		/// </param>
        [JsonConstructor]
        public QuickReply(string payload) {
            Payload = payload;
        }

        /// <summary>
        /// Gets the message payload of the quick reply.
        /// </summary>
        [JsonPropertyName("payload")]
        public string Payload { get; }
    }
}