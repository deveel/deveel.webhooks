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
    /// Represents an attachment to a message that carries an asset.
    /// </summary>
    /// <remarks>
    /// An asset attachment can be of type <see cref="AttachmentType.File"/>,
    /// <see cref="AttachmentType.Audio"/>, <see cref="AttachmentType.Video"/> or
    /// <see cref="AttachmentType.Image"/>.
    /// </remarks>
    public class AssetAttachment : Attachment {
        /// <summary>
        /// Initializes the attachment with the given type and payload.
        /// </summary>
        /// <param name="type">The type of asset (either file, video, audio, video)</param>
        /// <param name="payload">The payload describing the references to
        /// the file asset</param>
        /// <exception cref="ArgumentException">
        /// Thrown if the given <paramref name="type"/> is a <see cref="AttachmentType.Template"/>,
        /// that is not a valid type for this type of attachment (see <see cref="TemplateAttachment"/>).
        /// </exception>
        /// <exception cref="ArgumentNullException">
        /// Thrown if the payload is <c>null</c>.
        /// </exception>
        /// <exception cref="ArgumentException">
        /// Thrown if the given <paramref name="type"/> is a <see cref="AttachmentType.Template"/>
        /// or <see cref="AttachmentType.Fallback"/>
        /// </exception>
        [JsonConstructor]
        public AssetAttachment(AttachmentType type, UrlPayload payload) : base(type) {
            if (type == AttachmentType.Template ||
                type == AttachmentType.Fallback)
                throw new ArgumentException("The template type is not valid", nameof(type));

            Payload = payload ?? throw new ArgumentNullException(nameof(payload));
        }

        /// <summary>
        /// Gets the payload describing the references to the file asset.
        /// </summary>
        [JsonPropertyName("payload")]
        public UrlPayload Payload { get; }
    }
}