using System.Runtime.Serialization;

namespace Deveel.Facebook {
	/// <summary>
	/// Enumerates the types of attachments that can be sent
	/// </summary>
	public enum AttachmentType {
		/// <summary>
		/// The attachment is an image
		/// </summary>
		[EnumMember(Value = AttachmentTypeNames.Image)]
		Image,

		/// <summary>
		/// The attachment is an audio file
		/// </summary>
		[EnumMember(Value = AttachmentTypeNames.Audio)]
		Audio,

		/// <summary>
		/// The attachment is a video file
		/// </summary>
		[EnumMember(Value = AttachmentTypeNames.Video)]
		Video,

		/// <summary>
		/// The attachment is a generic file
		/// </summary>
		[EnumMember(Value = AttachmentTypeNames.File)]
		File,

		/// <summary>
		/// The attachment is a templated message
		/// </summary>
		[EnumMember(Value = AttachmentTypeNames.Template)]
		Template
	}
}
