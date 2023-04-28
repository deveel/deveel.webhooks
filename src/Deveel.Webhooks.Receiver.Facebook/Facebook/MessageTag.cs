using System;
using System.Runtime.Serialization;

namespace Deveel.Facebook {
	public enum MessageTag {
		[EnumMember(Value = "CONFIRMED_EVENT_UPDATE")]
		ConfirmedEventUpdate,

		[EnumMember(Value = "POST_PURCHASE_UPDATE")]
		PostPurchaseUpdate,

		[EnumMember(Value = "ACCOUNT_UPDATE")]
		AccountUpdate,

		[EnumMember(Value = "HUMAN_AGENT")]
		HumanAgent,

		[EnumMember(Value = "CUSTOMER_FEEDBACK")]
		CustomerFeedback
	}
}
