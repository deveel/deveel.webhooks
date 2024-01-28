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

using System.Runtime.Serialization;

namespace Deveel.Webhooks.Facebook
{
    /// <summary>
    /// Enumerates the types of attachments that can be sent
    /// </summary>
    public enum AttachmentType
    {
        /// <summary>
        /// The attachment is an image
        /// </summary>
        [EnumMember(Value = AttachmentTypeNames.Image)]
        Image,

        /// <summary>
        /// The attachment is an audio file
        /// </summary>
        [EnumMember(Value = AttachmentTypeNames.Audio)]
        Audio,

        /// <summary>
        /// The attachment is a video file
        /// </summary>
        [EnumMember(Value = AttachmentTypeNames.Video)]
        Video,

        /// <summary>
        /// The attachment is a generic file
        /// </summary>
        [EnumMember(Value = AttachmentTypeNames.File)]
        File,

        /// <summary>
        /// The attachment is a templated message
        /// </summary>
        [EnumMember(Value = AttachmentTypeNames.Template)]
        Template,

        /// <summary>
        /// The attachment is a fallback
        /// </summary>
        [EnumMember(Value = AttachmentTypeNames.Fallback)]
        Fallback
    }
}
