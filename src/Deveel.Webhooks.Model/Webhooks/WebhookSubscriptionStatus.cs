using System;
using System.Runtime.Serialization;

namespace Deveel.Webhooks {
	public enum WebhookSubscriptionStatus {
		[EnumMember(Value = "none")]
		None = 0,

		[EnumMember(Value = "active")]
		Active = 1,

		[EnumMember(Value = "suspended")]
		Suspended = 2,
	}
}
