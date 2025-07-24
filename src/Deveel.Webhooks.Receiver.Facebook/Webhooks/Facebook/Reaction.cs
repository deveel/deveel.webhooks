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
    /// Represents a reaction by a user to a message.
    /// </summary>
    public sealed record class Reaction {
        /// <summary>
        /// Constructs the reaction with the given message id, action and type.
        /// </summary>
        /// <param name="messageId">
        /// The identifier of the message that the reaction is related to.
        /// </param>
        /// <param name="action">
        /// The action that the user has performed on the message.
        /// </param>
        /// <param name="type">
        /// The type of reaction that the user has performed.
        /// </param>
        /// <exception cref="ArgumentException"></exception>
        [JsonConstructor]
        public Reaction(string messageId, ReactionActionType action, ReactionType type) {
            if (String.IsNullOrWhiteSpace(messageId))
                throw new ArgumentException($"'{nameof(messageId)}' cannot be null or whitespace.", nameof(messageId));

            MessageId = messageId;
            Action = action;
            Type = type;
        }

        /// <summary>
        /// Gets the type of reaction that the user has performed.
        /// </summary>
        [JsonPropertyName("reaction")]
        public ReactionType Type { get; }

        /// <summary>
        /// Gets or sets the code of the emoji that the user has used to
        /// </summary>
        [JsonPropertyName("emoji")]
        public string? Emoji { get; set; }

        /// <summary>
        /// Gets the action that the user has performed on the message.
        /// </summary>
        [JsonPropertyName("action")]
        public ReactionActionType Action { get; }

        /// <summary>
        /// Gets the identifier of the message that the reaction is related to.
        /// </summary>
        [JsonPropertyName("mid")]
        public string MessageId { get; }
    }
}
