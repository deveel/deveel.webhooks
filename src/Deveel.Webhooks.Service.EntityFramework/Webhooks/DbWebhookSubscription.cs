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

using System.Diagnostics.CodeAnalysis;

namespace Deveel.Webhooks {
    public class DbWebhookSubscription : IWebhookSubscription {
        public string? Id { get; set;}

        string? IWebhookSubscription.SubscriptionId => Id;

        public string? TenantId { get; set; }

        public string Name { get; set; }

        public string DestinationUrl { get; set; }

        public string? Secret { get; set; }

        public string? Format { get; set; }

        public WebhookSubscriptionStatus Status { get; set; }

        public int? RetryCount { get; set; }

        public DateTimeOffset? CreatedAt { get; set; }

        public DateTimeOffset? UpdatedAt { get; set; }

        IEnumerable<string> IWebhookSubscription.EventTypes =>
            Events?.Select(x => x.EventType) ?? Array.Empty<string>();

        public virtual List<DbWebhookSubscriptionEvent> Events { get; set; }

        IEnumerable<IWebhookFilter>? IWebhookSubscription.Filters => Filters.AsReadOnly();

        public List<DbWebhookFilter> Filters { get; set; }

        IDictionary<string, string>? IWebhookSubscription.Headers =>
            Headers?.ToDictionary(x => x.Key, x => x.Value);

		[ExcludeFromCodeCoverage]
        IDictionary<string, object>? IWebhookSubscription.Properties =>
            Properties?.ToDictionary(x => x.Key, x => DbWebhookValueConvert.Convert(x.Value, x.ValueType));

        public virtual List<DbWebhookSubscriptionHeader> Headers { get; set; }

        public virtual List<DbWebhookSubscriptionProperty> Properties { get; set; }
    }
}
