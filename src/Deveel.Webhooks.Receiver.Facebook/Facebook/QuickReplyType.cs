using System;
using System.Runtime.Serialization;

namespace Deveel.Facebook {
	public enum QuickReplyType {
		[EnumMember(Value = "text")]
		Text,

		[EnumMember(Value = "user_phone_number")]
		UserPhoneNumber,

		[EnumMember(Value = "user_email")]
		UserEmail
	}
}
