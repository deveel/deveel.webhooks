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

using System;
using System.Text.Json.Serialization;

namespace Deveel.Webhooks.Facebook
{
    /// <summary>
    /// A webhook sent from the Facebook Messenger Platform
    /// </summary>
    public sealed class FacebookWebhook
    {
        /// <summary>
        /// Constructs a new instance of the webhook object
        /// </summary>
        /// <param name="object">The type of object the webhook
        /// relates to.</param>
        [JsonConstructor]
        public FacebookWebhook(string @object)
        {
            Object = @object;
        }

        // TODO: use an enumeration
        /// <summary>
        /// Gets the type of object this webhook relates to.
        /// </summary>
        [JsonPropertyName("object")]
        public string Object { get; }

        /// <summary>
        /// Gets an array of entries
        /// </summary>
        [JsonPropertyName("entry")]
        public FacebookWebhookEntry[]? Entries { get; set; }
    }
}
