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

namespace Deveel.Webhooks.Facebook {
    /// <summary>
    /// References a person that sent a message to a business.
    /// </summary>
    public readonly struct SenderPart {
        /// <summary>
        /// Constructs the sender reference with the given identifier.
        /// </summary>
        /// <param name="id">
        /// The Page-scoped ID for the person who sent a message to your business
        /// </param>
        /// <param name="userRef"></param>
        [JsonConstructor]
        public SenderPart(string id, string? userRef = null) : this() {
            Id = id;
            UserRef = userRef;
        }

        /// <summary>
        /// Gets the page-scoped ID for the person who sent a message to your business
        /// </summary>
        [JsonPropertyName("id")]
        public string Id { get; }

        /// <summary>
        /// The reference for a person who used the <c>Chat Plugin</c> to 
        /// messsage your business.
        /// </summary>
        [JsonPropertyName("user_ref")]
        public string? UserRef { get; }

        /// <summary>
        /// Implicitly converts the given <paramref name="id"/> to a
        /// <see name="SenderPart"/> instance.
        /// </summary>
        /// <param name="id">
        /// The unique identifier of the sender.
        /// </param>
        public static implicit operator SenderPart(string id) => new SenderPart(id);
    }
}
