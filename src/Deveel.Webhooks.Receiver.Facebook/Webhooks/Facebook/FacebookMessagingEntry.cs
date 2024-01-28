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
    /// Represents a single entry in a Facebook Messenger Webhook
    /// </summary>
    public sealed class FacebookMessagingEntry {
        /// <summary>
        /// Constructs a new instance of the entry with the given
        /// parts that identify the sender and the recipient of the entry
        /// </summary>
        /// <param name="sender">
        /// The unique identifier in the scope of the Facebook Page (PSID)
        /// of the sender of the entry
        /// </param>
        /// <param name="recipient">
        /// The unique identifier of the Facebook Page (PAGE_ID) that
        /// received the entry.
        /// </param>
        [JsonConstructor]
        public FacebookMessagingEntry(SenderPart sender, MessagingPart recipient) {
            Sender = sender;
            Recipient = recipient;
        }

        /// <summary>
        /// Gets the unique identifier of the sender of the entry
        /// in the scope of the Facebook Page (PSID).
        /// </summary>
        [JsonPropertyName("sender")]
        public SenderPart Sender { get; }

        /// <summary>
        /// Gets the identifier of the Facebook Page that received
        /// the entry.
        /// </summary>
        [JsonPropertyName("recipient")]
        public MessagingPart Recipient { get; }

        /// <summary>
        /// Gets or sets an object that indicates the delivery status
        /// of messages sent from the page.
        /// </summary>
        [JsonPropertyName("delivery")]
        public MessageDelivery? Delivery { get; set; }

        /// <summary>
        /// Gets or sets an object that indicates the read status
        /// of the messages sent from the page.
        /// </summary>
        [JsonPropertyName("read")]
        public MessageRead? Read { get; set; }

        /// <summary>
        /// Gets or sets an object that describes a message received
        /// by a page by the given sender.
        /// </summary>
        [JsonPropertyName("message")]
        public ReceivedMessage? Message { get; set; }

        /// <summary>
        /// Gets or sets an object that indicates the webhook 
        /// is an opt-in/opt-out for messaging events.
        /// </summary>
        [JsonPropertyName("optin")]
        public MessagingOptIn? OptIn { get; set; }

        /// <summary>
        /// Gets or sets an object that indicates that a
        /// user played a round of an instant game.
        /// </summary>
        [JsonPropertyName("game_play")]
        public GamePlay? GamePlay { get; set; }

        /// <summary>
        /// Gets or sets an object that indicates the webhook
        /// is a reaction to a message.
        /// </summary>
        [JsonPropertyName("reaction")]
        public Reaction? Reaction { get; set; }
    }
}