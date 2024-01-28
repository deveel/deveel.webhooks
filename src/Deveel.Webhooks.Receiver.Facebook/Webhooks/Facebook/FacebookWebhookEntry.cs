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

using Deveel.Json;

namespace Deveel.Webhooks.Facebook {
    /// <summary>
    /// A single entry in a Facebook Webhook that
    /// provides information about a specific event
    /// </summary>
    public sealed class FacebookWebhookEntry {
        /// <summary>
        /// Constructs a new instance of the entry
        /// </summary>
        /// <param name="id">The unique identifier of the
        /// object associated to the entry</param>
        /// <param name="timeStamp">The time-stamp of the entry</param>
        /// <exception cref="ArgumentException">
        /// Thrown if the given <paramref name="id"/> is <c>null</c> or empty.
        /// </exception>
        [JsonConstructor]
        public FacebookWebhookEntry(string id, DateTimeOffset timeStamp) {
            if (string.IsNullOrWhiteSpace(id))
                throw new ArgumentException($"'{nameof(id)}' cannot be null or whitespace.", nameof(id));

            Id = id;
            TimeStamp = timeStamp;
        }

        /// <summary>
        /// Gets the unique identifier of the object associated 
        /// to the entry.
        /// </summary>
        [JsonPropertyName("id")]
        public string Id { get; }

        /// <summary>
        /// Gets the time-stamp of the entry.
        /// </summary>
        [JsonPropertyName("time")]
        [JsonConverter(typeof(UnixTimeMillisConverter))]
        public DateTimeOffset TimeStamp { get; }

        /// <summary>
        /// Gets a list of messaging entries
        /// </summary>
        [JsonPropertyName("messaging")]
        public IList<FacebookMessagingEntry>? Messaging { get; set; }
    }
}