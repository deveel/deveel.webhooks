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

using System.ComponentModel.DataAnnotations.Schema;

using MongoDB.Bson;

namespace Deveel.Webhooks {
	/// <summary>
	/// An implementation of the <see cref="IWebhook"/> that is backed by a MongoDB database.
	/// </summary>
	public class MongoWebhook : IWebhook {
		string IWebhook.Id => WebhookId;

		/// <summary>
		/// Gets or sets the identifier of the event / webhook notified.
		/// </summary>
		[Column("webhook_id")]
		public string WebhookId { get; set; }

		/// <summary>
		/// Gets or sets the timestamp of the event.
		/// </summary>
		[Column("timestamp")]
		public DateTimeOffset TimeStamp { get; set; }

		/// <summary>
		/// Gets or sets the type of the event.
		/// </summary>
		[Column("event_type")]
		public string EventType { get; set; }

		// TODO: convert this to a dynamic object?
		object IWebhook.Data => Data;

		/// <summary>
		/// Gets or sets the data of the event.
		/// </summary>
		[Column("data")]
		public BsonDocument Data { get; set; }
	}
}
