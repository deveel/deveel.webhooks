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
    /// Represents a fallback attachment that might include
    /// shared content from a person.
    /// </summary>
    public sealed class FallbackAttachment : Attachment {
        /// <summary>
        /// Constructs the attachment with the given payload.
        /// </summary>
        /// <param name="payload">
        /// The fallback payload that describes the content
        /// </param>
        public FallbackAttachment(FallbackPayload? payload) : base(AttachmentType.Fallback) {
            Payload = payload;
        }

        /// <summary>
        /// Gets the payload of the attachment.
        /// </summary>
        [JsonPropertyName("payload")]
        public FallbackPayload? Payload { get; }
    }
}
