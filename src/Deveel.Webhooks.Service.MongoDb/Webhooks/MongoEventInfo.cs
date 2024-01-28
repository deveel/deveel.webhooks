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

using System.ComponentModel.DataAnnotations.Schema;

using MongoDB.Bson;

namespace Deveel.Webhooks {
	/// <summary>
	/// Provides an implementation of <see cref="IEventInfo"/> that is
	/// capable of being stored in a MongoDB database.
	/// </summary>
	public class MongoEventInfo : IEventInfo {
		/// <inheritdoc/>
		[Column("subject")]
		public string Subject { get; set; }

		/// <inheritdoc/>
		[Column("event_type")]
		public string EventType { get; set; }

		/// <inheritdoc/>
		[Column("event_id")]
		public string EventId { get; set; }

		string IEventInfo.Id => EventId;

		/// <inheritdoc/>
		[Column("timestamp")]
		public DateTimeOffset TimeStamp { get; set; }

		/// <inheritdoc/>
		[Column("data_version")]
		public string? DataVersion { get; set; }

		object? IEventInfo.Data => EventData;

		/// <inheritdoc/>
		[Column("data")]
		public virtual BsonDocument EventData { get; set; }
	}
}
