using System.Runtime.Serialization;

namespace Deveel.Facebook {
	public enum SenderActionType {
		[EnumMember(Value = "typing_on")]
		TypingOn,

		[EnumMember(Value = "typing_off")]
		TypingOff,

		[EnumMember(Value = "mark_seen")]
		MarkSeen
	}
}
