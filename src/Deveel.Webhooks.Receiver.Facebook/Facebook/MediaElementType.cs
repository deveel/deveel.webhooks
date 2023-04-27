using System.Runtime.Serialization;

namespace Deveel.Facebook {
	public enum MediaElementType {
		[EnumMember(Value = "image")]
		Image,

		[EnumMember(Value = "video")]
		Video
	}
}
