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

namespace Deveel.Webhooks {
    // [Table("webhook_subscription_properties")]
    public class DbWebhookSubscriptionProperty {
        //[Key, DatabaseGenerated(DatabaseGeneratedOption.Identity), Column("id")]
        public int Id { get; set; }

        //[Required, Column("key")]
        public string Key { get; set; }

        //[Column("value")]
        public string? Value { get; set; }

        //[Required, Column("value_type")]
        public string ValueType { get; set; }

        // [ForeignKey(nameof(SubscriptionId))]
        public virtual DbWebhookSubscription? Subscription { get; set; }

        // [Required, Column("subscription_id")]
        public string? SubscriptionId { get; set; }

        public object? GetValue() {
            if (Value == null)
                return null;

            return ValueType switch {
                "string" => Value,
                "int" => int.Parse(Value),
                "bool" => bool.Parse(Value),
                "number" => double.Parse(Value),
                _ => Value
            };
        }
    }
}
