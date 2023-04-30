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

namespace Deveel.Webhooks {
	/// <summary>
	/// Provides information on an event that might
	/// trigger some notifications
	/// </summary>
	public readonly struct EventInfo : IEventInfo {
		/// <summary>
		/// Constructs an EventInfo instance of the given type
		/// and providing the given data
		/// </summary>
		/// <param name="subject">
		/// The subject of the event (e.g. the name of the aggregate)
		/// </param>
		/// <param name="eventType">
		/// The type of event that was triggered
		/// </param>
		/// <param name="dataVersion">
		/// The version of the data carried by the event
		/// </param>
		/// <param name="data">
		/// The data carried by the event
		/// </param>
		/// <param name="id">
		/// An optional unique identifier of the event
		/// </param>
		/// <param name="timeStamp">
		/// An optional time-stamp of the time the event occurred. When not
		/// provided the event is assumed to be occurred at the time of the
		/// initialization of this instance.
		/// </param>
		/// <exception cref="ArgumentException">
		/// If either the <paramref name="eventType"/> or <paramref name="subject"/> are 
		/// <c>null</c> or an empty string
		/// </exception>
		/// <exception cref="ArgumentNullException">
		/// If the data provided are null
		/// </exception>
		public EventInfo(string subject, string eventType, string? dataVersion = null, object? data = null, string? id = null, DateTimeOffset? timeStamp = null) {
			if (string.IsNullOrEmpty(subject)) 
				throw new ArgumentException($"'{nameof(subject)}' cannot be null or empty.", nameof(subject));

			if (string.IsNullOrWhiteSpace(eventType))
				throw new ArgumentException($"'{nameof(eventType)}' cannot be null or whitespace.", nameof(eventType));

			Subject = subject;
			EventType = eventType;
			DataVersion = dataVersion;
			Data = data ?? new object();

			Id = String.IsNullOrWhiteSpace(id) ? Guid.NewGuid().ToString("N") : id;
			TimeStamp = timeStamp ?? DateTimeOffset.UtcNow;
		}

		/// <summary>
		/// Gets the subject of the event (e.g. the name of the aggregate)
		/// </summary>
		public string Subject { get; }

		/// <summary>
		/// Gets the type of event
		/// </summary>
		public string EventType { get; }

		/// <summary>
		/// Gets or sets an unique identifier of the event 
		/// (by default, a new GUID)
		/// </summary>
		public string Id { get; }

		/// <summary>
		/// Gets or sets the exact time-stamp of the event
		/// (by default, the time of the creation of this instance)
		/// </summary>
		public DateTimeOffset TimeStamp { get; }

		/// <summary>
		/// Gets the version of the data carried by the event
		/// </summary>
		public string? DataVersion { get; }

		/// <summary>
		/// Gets the data transported by the event.
		/// </summary>
		public object? Data { get; }
	}
}
