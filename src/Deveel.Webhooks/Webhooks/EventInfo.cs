// Copyright 2022 Deveel
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

namespace Deveel.Webhooks {
	/// <summary>
	/// Provides information on an event that might
	/// trigger some notifications
	/// </summary>
	public sealed class EventInfo {
		/// <summary>
		/// Constructs an EventInfo instance of the given type
		/// and providing the given data
		/// </summary>
		/// <param name="eventType">The type of event</param>
		/// <param name="data">The data provided by the event</param>
		/// <exception cref="ArgumentException">
		/// If the <param name="eventType">event type</param> is null or an empty string
		/// </exception>
		/// <exception cref="ArgumentNullException">
		/// If the data provided are null
		/// </exception>
		public EventInfo(string eventType, object data) {
			if (string.IsNullOrWhiteSpace(eventType))
				throw new ArgumentException($"'{nameof(eventType)}' cannot be null or whitespace.", nameof(eventType));
			if (data is null)
				throw new ArgumentNullException(nameof(data));

			EventType = eventType;
			Data = data;

			Id = Guid.NewGuid().ToString("N");
			TimeStamp = DateTimeOffset.UtcNow;
		}

		/// <summary>
		/// Gets the type of event
		/// </summary>
		public string EventType { get; }

		/// <summary>
		/// Gets or sets an unique identifier of the event 
		/// (by default, a new GUID)
		/// </summary>
		public string Id { get; set; }

		/// <summary>
		/// Gets or sets the exact time-stamp of the event
		/// (by default, the time of the creation of this instance)
		/// </summary>
		public DateTimeOffset TimeStamp { get; set; }

		/// <summary>
		/// Gets the data transported by the event.
		/// </summary>
		public object Data { get; private set; }

		/// <summary>
		/// Updates the data of the event
		/// </summary>
		/// <param name="data">The object carrying the new
		/// data of the event</param>
		/// <exception cref="ArgumentNullException"></exception>
		public void SetData(object data) {
			if (data is null)
				throw new ArgumentNullException(nameof(data));

			Data = data;
		}
	}
}
