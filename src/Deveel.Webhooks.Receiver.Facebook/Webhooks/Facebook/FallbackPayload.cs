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
    /// Describes the payload of a fallback attachment.
    /// </summary>
    public sealed record class FallbackPayload {
        /// <summary>
        /// Initializes the payload with the given URL.
        /// </summary>
        /// <param name="url">
        /// The URL of the item attached to the fallback.
        /// </param>
        [JsonConstructor]
        public FallbackPayload(string url) {
            Url = url;
        }

        /// <summary>
        /// Gets the URL of the item attached to the fallback.
        /// </summary>
        [JsonPropertyName("url")]
        public string Url { get; }

        /// <summary>
        /// Gets or sets the title of the item attached to the fallback.
        /// </summary>
        [JsonPropertyName("title")]
        public string? Title { get; set; }
    }
}
