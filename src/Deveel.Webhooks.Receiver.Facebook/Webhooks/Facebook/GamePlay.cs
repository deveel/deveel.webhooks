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

using System.Text.Json.Nodes;
using System.Text.Json.Serialization;

namespace Deveel.Webhooks.Facebook {
    /// <summary>
    /// Represents a round of a game played by a user.
    /// </summary>
    public sealed record class GamePlay {
        /// <summary>
        /// Constructs the game play information.
        /// </summary>
        /// <param name="gameId">
        /// The identifier of the game that was played (the Meta App ID).
        /// </param>
        /// <param name="playerId">
        /// The identifier of the player that played the game.
        /// </param>
        /// <param name="contextType">
        /// The type of the context in which the game was played.
        /// </param>
        [JsonConstructor]
        public GamePlay(string gameId, string playerId, GameContextType contextType) {
            GameId = gameId;
            PlayerId = playerId;
            ContextType = contextType;
        }

        /// <summary>
        /// Gets the identifier of the game that was played.
        /// </summary>
        [JsonPropertyName("game_id")]
        public string GameId { get; }

        /// <summary>
        /// Gets the identifier of the player that played the game.
        /// </summary>
        [JsonPropertyName("player_id")]
        public string PlayerId { get; }

        /// <summary>
        /// Gets the type of the context in which the game was played.
        /// </summary>
        [JsonPropertyName("context_type")]
        public GameContextType ContextType { get; }

        /// <summary>
        /// Gets or sets the identifier for the social context type if the type 
        /// is not <see cref="GameContextType.Solo"/>.
        /// </summary>
        [JsonPropertyName("context_id")]
        public string? ContextId { get; set; }

        /// <summary>
        /// Gets or sets the locale of the player that played the game.
        /// </summary>
        [JsonPropertyName("locale")]
        public string? Locale { get; set; }

        /// <summary>
        /// Gets or sets the JSON encoded object that represents the session
        /// data of the game (only available for Rich Games)
        /// </summary>
        // TODO: Should this be a JSON object?
        [JsonPropertyName("payload")]
        public JsonObject? Payload { get; set; }

        /// <summary>
        /// Gets or sets the best score achieved by this playing during 
        /// this round of game play (only available to Classic score 
        /// based games).
        /// </summary>
        [JsonPropertyName("score")]
        public int? Score { get; set; }
    }
}
