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

// Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
#pragma warning disable CS8618

namespace Deveel.Webhooks {
	/// <summary>
	/// Implements the <see cref="IWebhookDeliveryResult"/> interface to
	/// allow the storage of the result of a delivery attempt in a database
	/// </summary>
    public class DbWebhookDeliveryResult : IWebhookDeliveryResult {
		/// <summary>
		/// Gets or sets the database identifier of the delivery result.
		/// </summary>
        public int? Id { get; set; }

		/// <inheritdoc/>
        public string OperationId { get; set; }

		/// <summary>
		/// Gets or sets the identifier of the tenant that owns the webhook
		/// </summary>
		public string? TenantId { get; set; }

        IEventInfo IWebhookDeliveryResult.EventInfo => EventInfo;

		/// <summary>
		/// Gets or sets the database entity that describes the event
		/// that triggered the delivery of the webhook.
		/// </summary>
        public virtual DbEventInfo EventInfo { get; set; }

		/// <summary>
		/// Gets or sets the database identifier of the event that triggered
		/// the delivery of the webhook.
		/// </summary>
        public int? EventId { get; set; }

		/// <summary>
		/// Gets or sets the database entity that describes the webhook
		/// that was delivered.
		/// </summary>
        public virtual DbWebhook Webhook { get; set; }

		/// <summary>
		/// Gets or sets the database identifier of the webhook that was delivered
		/// </summary>
        public int? WebhookId { get; set; }

        IWebhook IWebhookDeliveryResult.Webhook => Webhook;

        IWebhookReceiver IWebhookDeliveryResult.Receiver => Receiver;

		/// <summary>
		/// Gets or sets the database entity that describes the receiver
		/// of the webhook.
		/// </summary>
        public virtual DbWebhookReceiver Receiver { get; set; }

		/// <summary>
		/// Gets or sets the database identifier of the receiver of the webhook.
		/// </summary>
        public int? ReceiverId { get; set; }

        IEnumerable<IWebhookDeliveryAttempt> IWebhookDeliveryResult.DeliveryAttempts => DeliveryAttempts.AsReadOnly();

		/// <summary>
		/// Gets or sets the list of delivery attempts that were made to deliver
		/// the webhook.
		/// </summary>
        public virtual List<DbWebhookDeliveryAttempt> DeliveryAttempts { get; set; }
    }
}
