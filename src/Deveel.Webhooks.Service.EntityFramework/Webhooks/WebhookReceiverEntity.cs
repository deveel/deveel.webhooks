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
    [Table("webhook_receivers")]
    public class WebhookReceiverEntity : IWebhookReceiver {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity), Column("id")]
        public int? Id { get; set; }

        [Column("subscription_id")]
        public string? SubscriptionId { get; set; }

        public virtual WebhookSubscriptionEntity? Subscription { get; set; }

        [Column("name")]
        public string? SubscriptionName { get; set; }

        [Required, Column("url")]
        public string DestinationUrl { get; set; }

        IEnumerable<KeyValuePair<string, string>> IWebhookReceiver.Headers
            => Headers?.ToDictionary(x => x.Key, y => y.Value);

        public virtual List<WebhookReceiverHeader> Headers { get; set; }

        [Column("body_format")]
        public string BodyFormat { get; set; }
    }
}
