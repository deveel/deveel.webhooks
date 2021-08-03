using System;
using System.Collections.Generic;

using Deveel.Events;

namespace Deveel.Webhooks {
	public sealed class WebhookNotification : Event {
		public WebhookNotification(string eventId, string eventType, object data)
			: base(eventId, eventType, data) {
		}

		public WebhookNotification(string eventType, object data)
			: base(eventType, data) {
		}

	}
}
