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

using System;
using System.Text.Json.Serialization;

namespace Deveel.Webhooks.Facebook {
    /// <summary>
    /// Represents a message received from a user
    /// to a Facebook Messenger application.
    /// </summary>
    public sealed class ReceivedMessage {
        /// <summary>
        /// Constructs the message with the given identifier.
        /// </summary>
        /// <param name="id"></param>
        [JsonConstructor]
        public ReceivedMessage(string id) {
            Id = id;
        }

        /// <summary>
        /// Gets the unique identifier of the message.
        /// </summary>
        [JsonPropertyName("mid")]
        public string Id { get; }

        /// <summary>
        /// Gets the reference to a message that this
        /// was a reply to.
        /// </summary>
        [JsonPropertyName("reply_to")]
        public MessageRef? ReplyTo { get; set; }

        /// <summary>
        /// Gets the content of a text message.
        /// </summary>
        [JsonPropertyName("text")]
        public string? Text { get; set; }

        /// <summary>
        /// Gets a set of attachments to the message,
        /// that can be images, videos, audio or files
        /// or templates.
        /// </summary>
        [JsonPropertyName("attachments")]
        public Attachment[]? Attachments { get; set; }

        /// <summary>
        /// Gets a quick reply to the message.
        /// </summary>
        [JsonPropertyName("quick_reply")]
        public QuickReply? QuickReply { get; set; }
    }
}