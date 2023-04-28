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
    /// Represents the reference to a part in a messaging event.
    /// </summary>
    public readonly struct MessagingPart {
        /// <summary>
        /// Initializes the part with the given identifier.
        /// </summary>
        /// <param name="id"></param>
        /// <exception cref="ArgumentException">
        /// Thrown when the given <paramref name="id"/> is <c>null</c> or whitespace.
        /// </exception>
        [JsonConstructor]
        public MessagingPart(string id) {
            if (String.IsNullOrWhiteSpace(id))
                throw new ArgumentException($"'{nameof(id)}' cannot be null or whitespace.", nameof(id));

            Id = id;
        }

        /// <summary>
        /// Gets the identifier of the part.
        /// </summary>
        [JsonPropertyName("id")]
        public string Id { get; }

        /// <summary>
        /// Implicitly converts the part to its identifier.
        /// </summary>
        /// <param name="part"></param>
        public static implicit operator string(MessagingPart part) => part.Id;

        /// <summary>
        /// Implicitly converts the given identifier to a part.
        /// </summary>
        /// <param name="id"></param>
        public static implicit operator MessagingPart(string id) => new MessagingPart(id);
    }
}