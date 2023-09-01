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
    [Table("webhook_delivery_results")]
    public class WebhookDeliveryResultEntity : IWebhookDeliveryResult {
        [Key, Column("id"), DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int? Id { get; set; }

        [Required, Column("operation_id")]
        public string OperationId { get; set; }

        IEventInfo IWebhookDeliveryResult.EventInfo => EventInfo;

        public virtual EventInfoEntity EventInfo { get; set; }

        [Required, Column("event_id")]
        public string EventId { get; set; }

        public int? EventInfoId { get; set; }

        public virtual WebhookEntity Webhook { get; set; }

        public int? WebhookId { get; set; }

        IWebhook IWebhookDeliveryResult.Webhook => Webhook;

        IWebhookReceiver IWebhookDeliveryResult.Receiver => Receiver;

        public virtual WebhookReceiverEntity Receiver { get; set; }

        [Column("receiver_id")]
        public int? ReceiverId { get; set; }

        IEnumerable<IWebhookDeliveryAttempt> IWebhookDeliveryResult.DeliveryAttempts => DeliveryAttempts.AsReadOnly();

        public virtual List<WebhookDeliveryAttemptEntity> DeliveryAttempts { get; set; }
    }
}
