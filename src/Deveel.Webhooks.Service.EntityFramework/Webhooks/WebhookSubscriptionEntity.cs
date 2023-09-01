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
    [Table("webhook_subscriptions")]
    public class WebhookSubscriptionEntity : IWebhookSubscription {
        [Key, Column("id"), DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public string? Id { get; set;}

        string? IWebhookSubscription.SubscriptionId => Id;

        [Column("tenant_id")]
        public string? TenantId { get; set; }

        [Column("name"), Required]
        public string Name { get; set; }

        [Column("destination_url"), Required]
        public string DestinationUrl { get; set; }

        [Column("secret")]
        public string? Secret { get; set; }

        [Column("format")]
        public string? Format { get; set; }

        [Column("status")]
        public WebhookSubscriptionStatus Status { get; set; }

        [Column("retry_count")]
        public int? RetryCount { get; set; }


        [Column("created_at")]
        public DateTimeOffset? CreatedAt { get; set; }

        [Column("updated_at")]
        public DateTimeOffset? UpdatedAt { get; set; }

        IEnumerable<string> IWebhookSubscription.EventTypes =>
            Events?.Select(x => x.EventType) ?? Array.Empty<string>();

        public virtual List<WebhookEventSubscription> Events { get; set; }

        IEnumerable<IWebhookFilter>? IWebhookSubscription.Filters => Filters.AsReadOnly();

        public List<WebhookFilterEntity> Filters { get; set; }

        IDictionary<string, string>? IWebhookSubscription.Headers =>
            Headers?.ToDictionary(x => x.Key, x => x.Value);

        IDictionary<string, object>? IWebhookSubscription.Properties =>
            Properties?.ToDictionary(x => x.Key, x => x.GetValue());

        public virtual List<WebhookSubscriptionHeader> Headers { get; set; }

        public virtual List<WebhookSubscriptionProperty> Properties { get; set; }
    }
}
