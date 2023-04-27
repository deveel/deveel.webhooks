using System.Runtime.Serialization;

namespace Deveel.Facebook {
	public enum MessagingType {
		[EnumMember(Value = "RESPONSE")]
		Response,

		[EnumMember(Value = "UPDATE")]
		Update,

		[EnumMember(Value = "MESSAGE_TAG")]
		MessageTag
	}
}
