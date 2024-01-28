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

namespace Deveel.Webhooks.Facebook
{
    /// <summary>
    /// Represents an audio file attachment.
    /// </summary>
    public sealed class AudioAttachment : AssetAttachment
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="AudioAttachment"/> class.
        /// </summary>
        /// <param name="payload">The payload that describes the file references</param>
        [JsonConstructor]
        public AudioAttachment(UrlPayload payload)
            : base(AttachmentType.Audio, payload)
        {
        }
    }
}
