﻿// Copyright 2022-2023 Deveel
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
	/// <summary>
	/// An implementation of <see cref="IEventInfo"/> that is used to store
	/// the event information in a database.
	/// </summary>
    public class DbEventInfo : IEventInfo {
		/// <summary>
		/// Gets or sets the identifier of the event entity
		/// in the database.
		/// </summary>
        public int? Id { get; set; }

		/// <inheritdoc/>
        public string Subject { get; set; }

		/// <inheritdoc/>
        public string EventType { get; set;}

		/// <summary>
		/// Gets or sets the unique identifier of the event.
		/// </summary>
        public string EventId { get; set; }

        string IEventInfo.Id => EventId;

		/// <inheritdoc/>
        public DateTimeOffset TimeStamp { get; set; }

		/// <inheritdoc/>
        public string? DataVersion { get; set; }

        // TODO: Convert from JSON
        object? IEventInfo.Data => Data;

		/// <summary>
		/// Gets or sets the JSON representation of the event data.
		/// </summary>
        public string? Data { get; set; }

		/// <summary>
		/// Gets or sets the list of properties associated to the event.
		/// </summary>
		public virtual List<DbEventProperty> Properties { get; set; }

		IDictionary<string, object?> IEventInfo.Properties 
			=> Properties.ToDictionary(x => x.Key, x => x.GetValue());
    }
}
