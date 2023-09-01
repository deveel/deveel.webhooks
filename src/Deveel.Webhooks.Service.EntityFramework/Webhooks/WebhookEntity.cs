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

using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Deveel.Webhooks {
    /// <summary>
    /// The entity that represents a webhook in the database
    /// </summary>
    [Table("webhooks")]
    public class WebhookEntity : IWebhook {
        /// <summary>
        /// Gets or sets the unique identifier of the webhook
        /// </summary>
        [Key, Column("id"), DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int? Id { get; set; }

        [Column("webhook_id")]
        public string WebhookId { get; set; }

        string? IWebhook.Id => WebhookId;

        /// <summary>
        /// Gets or sets the exact time when the webhook was created
        /// </summary>
        [Required, Column("timestamp")]
        public DateTimeOffset TimeStamp { get; set; }

        /// <summary>
        /// Gets or sets the name of the event that triggered 
        /// the webhook
        /// </summary>
        [Required, Column("event_type")]
        public string EventType { get; set; }

        /// <summary>
        /// Gets or sets the payload data of the webhook (as a JSON string)
        /// </summary>
        [Column("data")]
        public string? Data { get; set; }

        object? IWebhook.Data => Data;
    }
}
