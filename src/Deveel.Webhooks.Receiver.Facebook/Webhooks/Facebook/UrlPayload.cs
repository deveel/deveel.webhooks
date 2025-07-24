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
    /// The payload of an attachment that contains an URL.
    /// </summary>
    public sealed class UrlPayload {
        /// <summary>
        /// Initializes a new instance of the <see cref="UrlPayload"/> class.
        /// </summary>
        /// <param name="url">
        /// The URL of the attachment that was sent.
        /// </param>
        /// <exception cref="ArgumentException">
        /// Thrown when the given <paramref name="url"/> is <c>null</c> or empty
        /// </exception>
        public UrlPayload(string url) {
            if (String.IsNullOrWhiteSpace(url))
                throw new ArgumentException($"'{nameof(url)}' cannot be null or whitespace.", nameof(url));

            Url = url;
        }

        /// <summary>
        /// Gets the URL of the attachment that was sent.
        /// </summary>
        [JsonPropertyName("url")]
        public string Url { get; }
    }
}
