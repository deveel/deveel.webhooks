using System;

namespace Deveel.Webhooks {
	[Flags]
	public enum WebhookFields {
		None = 0,
		Name = 1,
		EventId = 2,
		EventName = 4,
		TimeStamp = 8,
		All = Name | EventId | EventName | TimeStamp
	}
}
