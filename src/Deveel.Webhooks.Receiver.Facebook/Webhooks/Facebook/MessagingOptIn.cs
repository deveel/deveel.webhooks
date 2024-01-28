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
    /// Describes an event of a user opting-in or opting-out to 
    /// receive messages from a page.
    /// </summary>
    public sealed class MessagingOptIn {
        /// <summary>
        /// Constructs the messaging opt-in event of the
        /// given type.
        /// </summary>
        /// <param name="type">
        /// The type of the opt-in event (it should always
        /// be <c>notification_messages</c>)
        /// </param>
        [JsonConstructor]
        public MessagingOptIn(string type) {
            Type = type;
        }

        /// <summary>
        /// Gets the type of the opt-in event.
        /// </summary>
        [JsonPropertyName("type")]
        public string Type { get; }

        /// <summary>
        /// Gets or sets an external reference that the developer can use
        /// to correlate the opt-in event with a user action
        /// </summary>
        [JsonPropertyName("ref")]
        public string? Reference { get; set; }

        /// <summary>
        /// Gets or sets the token that represents the person who opted-in, 
        /// with the specific topic and message frequency, that is used to 
        /// send recurring notifications
        /// </summary>
        [JsonPropertyName("notification_messages_token")]
        public string? NotificationToken { get; set; }

        /// <summary>
        /// Gets or sets the frequency of the notifications to send to the user.
        /// </summary>
        [JsonPropertyName("notification_messages_frequency")]
        public NotificationFequency? Fequency { get; set; }

        /// <summary>
        /// Gets or sets the status of the notification subscription.
        /// </summary>
        [JsonPropertyName("notification_messages_status")]
        public NotificationStatus? Status { get; set; }

        /// <summary>
        /// Gets or sets the date when the the notification message token expires
        /// </summary>
        [JsonPropertyName("token_expiry_timestamp")]
        [JsonConverter(typeof(UnixTimeMillisConverter))]
        public DateTimeOffset? TokenExpiresAt { get; set; }

        /// <summary>
        /// Gets or sets the timezone of the user
        /// </summary>
        [JsonPropertyName("notification_messages_timezone")]
        public string? Timezone { get; set; }

        /// <summary>
        /// Gets or sets the topic for the recurring notification message
        /// </summary>
        [JsonPropertyName("topic")]
        public string? Topic { get; set; }

        /// <summary>
        /// Gets or sets the status of the user token.
        /// </summary>
        [JsonPropertyName("user_token_status")]
        public UserTokenStatus? UserTokenStatus { get; set; }

        /// <summary>
        /// Gets or sets additional information that the developers includes 
        /// in the webhooks notification
        /// </summary>
        [JsonPropertyName("payload")]
        public string? Payload { get; set; }
    }
}
