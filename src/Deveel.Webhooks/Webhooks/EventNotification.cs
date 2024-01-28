using System.Collections;
using System.Diagnostics.CodeAnalysis;

namespace Deveel.Webhooks {
	public sealed class EventNotification : IEnumerable<EventInfo> {
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
		}

		public EventNotification(EventInfo eventInfo)
			: this(eventInfo.EventType, new[] {eventInfo}) {
		}

		public IReadOnlyList<EventInfo> Events { get; }

		public bool HasSingleEvent => Events.Count == 1;

		public EventInfo SingleEvent {
			get {
				if (!HasSingleEvent)
					throw new InvalidOperationException("The notification has more than one event");

				return Events[0];
			}
		}

		public string EventType { get; }

		public string NotificationId { get; set; }

		public IDictionary<string, object> Properties { get; set; } = new Dictionary<string, object>();

		public IEnumerator<EventInfo> GetEnumerator() => Events.GetEnumerator();

		[ExcludeFromCodeCoverage]
		IEnumerator IEnumerable.GetEnumerator() => ((IEnumerable)Events).GetEnumerator();

		private static void ValidateAllEventsOfType(string eventType, IEnumerable<EventInfo> events) {
			foreach (var @event in events) {
				if (!String.Equals(@event.EventType, eventType, StringComparison.OrdinalIgnoreCase))
					throw new ArgumentException($"The event {@event.EventType} is not of the type {eventType}");
			}
		}
	}
}
