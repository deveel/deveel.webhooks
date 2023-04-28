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

using System.Text.Json.Serialization;

namespace Deveel.Webhooks.Facebook {
    /// <summary>
    /// A reference to another message in the same conversation.
    /// </summary>
    public readonly struct MessageRef {
        /// <summary>
        /// Constructs the reference to a message with the 
        /// given identifier.
        /// </summary>
        /// <param name="id">
        /// The identifier of the message to reference.
        /// </param>
        /// <exception cref="ArgumentException">
        /// Thrown when the given <paramref name="id"/> is <c>null</c> or empty.
        /// </exception>
        [JsonConstructor]
        public MessageRef(string id) {
            if (string.IsNullOrWhiteSpace(id))
                throw new ArgumentException($"'{nameof(id)}' cannot be null or whitespace.", nameof(id));

            Id = id;
        }

        /// <summary>
        /// Gets the identifier of the message to reference.
        /// </summary>
        [JsonPropertyName("mid")]
        public string Id { get; }

        /// <summary>
        /// Implicitly converts the given <paramref name="id"/> to a
        /// <see cref="MessageRef"/> instance
        /// </summary>
        /// <param name="id"></param>
        public static implicit operator MessageRef(string id) {
            return new MessageRef(id);
        }
    }
}
