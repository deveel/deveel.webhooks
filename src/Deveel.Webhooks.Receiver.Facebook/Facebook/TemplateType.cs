using System.Runtime.Serialization;

namespace Deveel.Facebook {
	public enum TemplateType {
		[EnumMember(Value = "generic")]
		Generic,

		[EnumMember(Value = "button")]
		Button,

		[EnumMember(Value = "media")]
		Media,

		[EnumMember(Value = "receipt")]
		Receipt,

		[EnumMember(Value = "product")]
		Product
	}
}
