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

using System.Collections;
using System.Diagnostics.CodeAnalysis;

namespace Deveel.Webhooks {
	/// <summary>
	/// Represents a notification of one or more events of the same type,
	/// to be delivered to a receiver.
	/// </summary>
	public sealed class EventNotification : IEnumerable<EventInfo> {
		/// <summary>
		/// Constructs the notification with the given type and events.
		/// </summary>
		/// <param name="eventType">
		/// The type of the events that are being notified.
		/// </param>
		/// <param name="events">
		/// The list of events that are being notified.
		/// </param>
		/// <exception cref="ArgumentException">
		/// Thrown when the given event type is null or empty, or when the list
		/// is empty, or when the list contains events that are not of the given type.
		/// </exception>
		public EventNotification(string eventType, IEnumerable<EventInfo> events) {
			ArgumentNullException.ThrowIfNull(events, nameof(events));

#if NET7_0_OR_GREATER
			ArgumentException.ThrowIfNullOrEmpty(eventType, nameof(eventType));
#else
			if (String.IsNullOrEmpty(eventType))
				throw new ArgumentException("The event type cannot be null or empty", nameof(eventType));
#endif
			if (!events.Any())
				throw new ArgumentException("The list of events cannot be empty", nameof(events));

			ValidateAllEventsOfType(eventType, events);

			Events = events.ToList().AsReadOnly();
			EventType = eventType;
			NotificationId = Guid.NewGuid().ToString();
			TimeStamp = DateTimeOffset.UtcNow;
		}

		/// <summary>
		/// Constructs the notification of a single event.
		/// </summary>
		/// <param name="eventInfo">
		/// The event that is being notified.
		/// </param>
		public EventNotification(EventInfo eventInfo)
			: this(eventInfo.EventType, new[] {eventInfo}) {
			NotificationId = eventInfo.Id;
			TimeStamp = eventInfo.TimeStamp;
		}

		/// <summary>
		/// Gets the list of events that are being notified.
		/// </summary>
		public IReadOnlyList<EventInfo> Events { get; }

		/// <summary>
		/// Gets a value indicating if the notification has a single event.
		/// </summary>
		public bool HasSingleEvent => Events.Count == 1;

		/// <summary>
		/// Gets the single event of the notification.
		/// </summary>
		/// <remarks>
		/// It is recommended to use this property only after checking that
		/// the notification has a single event, using the <see cref="HasSingleEvent"/>.
		/// </remarks>
		/// <exception cref="InvalidOperationException">
		/// Thrown when the notification has more than one event.
		/// </exception>
		/// <seealso cref="HasSingleEvent"/>
		public EventInfo SingleEvent {
			get {
				if (!HasSingleEvent)
					throw new InvalidOperationException("The notification has more than one event");

				return Events[0];
			}
		}

		/// <summary>
		/// Gets the type of the events that are being notified.
		/// </summary>
		public string EventType { get; }

		/// <summary>
		/// Gets or sets the unique identifier of the notification.
		/// </summary>
		public string NotificationId { get; set; }

		/// <summary>
		/// Gets or sets the timestamp of the notification.
		/// </summary>
		public DateTimeOffset TimeStamp { get; set; }

		/// <summary>
		/// Gets or sets the properties of the notification.
		/// </summary>
		public IDictionary<string, object> Properties { get; set; } = new Dictionary<string, object>();

		/// <inheritdoc/>
		public IEnumerator<EventInfo> GetEnumerator() => Events.GetEnumerator();

		[ExcludeFromCodeCoverage]
		IEnumerator IEnumerable.GetEnumerator() => ((IEnumerable)Events).GetEnumerator();

		private static void ValidateAllEventsOfType(string eventType, IEnumerable<EventInfo> events) {
			foreach (var @event in events) {
				if (!String.Equals(@event.EventType, eventType, StringComparison.OrdinalIgnoreCase))
					throw new ArgumentException($"The event {@event.EventType} is not of the type {eventType}");
			}
		}

		/// <summary>
		/// Implicitly converts an event into a notification.
		/// </summary>
		/// <param name="eventInfo">
		/// The event to be notified.
		/// </param>
		public static implicit operator EventNotification(EventInfo eventInfo) {
			return new EventNotification(eventInfo);
		}

		/// <summary>
		/// Implicitly converts a list of events into a notification.
		/// </summary>
		/// <param name="events">
		/// The list of events to be notified.
		/// </param>
		/// <exception cref="ArgumentException">
		/// Thrown if the list of events is empty.
		/// </exception>
		public static implicit operator EventNotification(EventInfo[] events) {
			if (events.Length == 0)
				throw new ArgumentException("The list of events cannot be empty", nameof(events));

			return new EventNotification(events[0].EventType, events);
		}
	}
}
