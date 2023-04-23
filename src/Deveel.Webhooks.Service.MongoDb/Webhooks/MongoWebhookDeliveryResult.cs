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
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

using MongoFramework;

namespace Deveel.Webhooks {
	[Table(MongoDbWebhookStorageConstants.DeliveryResultsCollectionName)]
	public class MongoWebhookDeliveryResult : IWebhookDeliveryResult {
		[BsonId, Key]
		public ObjectId Id { get; set; }

		IWebhook IWebhookDeliveryResult.Webhook => Webhook;

		public virtual MongoWebhookReceiver Receiver { get; set; }

		IWebhookReceiver IWebhookDeliveryResult.Receiver => Receiver;

		public virtual MongoWebhook Webhook { get; set; }

		IEnumerable<IWebhookDeliveryAttempt> IWebhookDeliveryResult.DeliveryAttempts => DeliveryAttempts;

		public virtual List<MongoWebhookDeliveryAttempt> DeliveryAttempts { get; set; }

		public string TenantId { get; set; }
	}
}
