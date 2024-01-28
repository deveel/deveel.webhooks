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

namespace Deveel.Webhooks.Facebook
{
    /// <summary>
    /// Describes an attachment to a message.
    /// </summary>
    [JsonConverter(typeof(PolymorphicJsonConverter<Attachment>))]
    [JsonDiscriminator("type")]
    [JsonKnownType(typeof(FileAttachment), AttachmentTypeNames.File)]
    [JsonKnownType(typeof(VideoAttachment), AttachmentTypeNames.Video)]
    [JsonKnownType(typeof(AudioAttachment), AttachmentTypeNames.Audio)]
    [JsonKnownType(typeof(ImageAttachment), AttachmentTypeNames.Image)]
    [JsonKnownType(typeof(TemplateAttachment), AttachmentTypeNames.Template)]
    [JsonKnownType(typeof(FallbackAttachment), AttachmentTypeNames.Fallback)]
    public abstract class Attachment
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Attachment"/> class
        /// </summary>
        /// <param name="type"></param>
        protected Attachment(AttachmentType type)
        {
            Type = type;
        }

        /// <summary>
        /// Gets the type of the attachment.
        /// </summary>
        [JsonPropertyName("type")]
        public AttachmentType Type { get; }
    }
}
