using System;

namespace Deveel.Webhooks {
	public sealed class EventInfo {
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

		public string EventType { get; }

		public string Id { get; set; }

		public DateTimeOffset TimeStamp { get; set; }

		public object Data { get; private set; }

		public void SetData(object data) {
			if (data is null)
				throw new ArgumentNullException(nameof(data));

			Data = data;
		}
	}
}
