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

// Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
#pragma warning disable CS8618

using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace Deveel.Webhooks {
	/// <summary>
	/// The model of a delivery result of a webhook that is
	/// stored in a MongoDB storage.
	/// </summary>
	[Table(MongoDbWebhookStorageConstants.DeliveryResultsCollectionName)]
	public class MongoWebhookDeliveryResult : IWebhookDeliveryResult {
		/// <summary>
		/// Gets or sets the identifier of the delivery result entity.
		/// </summary>
		[BsonId, Key]
		public ObjectId Id { get; set; }

		IWebhook IWebhookDeliveryResult.Webhook => Webhook;


        /// <inheritdoc/>
        public virtual MongoWebhookReceiver Receiver { get; set; }

		IWebhookReceiver IWebhookDeliveryResult.Receiver => Receiver;

        /// <inheritdoc/>
		public virtual MongoWebhook Webhook { get; set; }

        IEnumerable<IWebhookDeliveryAttempt> IWebhookDeliveryResult.DeliveryAttempts => DeliveryAttempts;

        /// <inheritdoc/>
        public virtual List<MongoWebhookDeliveryAttempt> DeliveryAttempts { get; set; }

		/// <summary>
		/// Gets or sets the unique identifier of the tenant that owns
		/// the webhook
		/// </summary>
		public string? TenantId { get; set; }
	}
}
