using System;

using Deveel.Events;

namespace Deveel.Webhooks {
	static class EventExtensions {
		public static EventInfo AsEventInfo(this IEvent @event)
			=> new EventInfo(@event.Type, @event.Data) {
				TimeStamp = @event.TimeStamp,
				Id = @event.Id
			};
	}
}
