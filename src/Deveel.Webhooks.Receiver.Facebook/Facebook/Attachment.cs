using System.Text.Json.Serialization;

using Deveel.Json;

namespace Deveel.Facebook {
	/// <summary>
	/// Describes an attachment to a message.
	/// </summary>
	[JsonConverter(typeof(PolymorphicJsonConverter<Attachment>))]
	[JsonDiscriminator("type")]
	[JsonKnownType(typeof(FileAttachment), AttachmentTypeNames.File)]
	[JsonKnownType(typeof(VideoAttachment), AttachmentTypeNames.Video)]
	[JsonKnownType(typeof(AudioAttachment), AttachmentTypeNames.Audio)]
	[JsonKnownType(typeof(ImageAttachment), AttachmentTypeNames.Image)]
	[JsonKnownType(typeof(TemplateAttachment), AttachmentTypeNames.Template)]
	public abstract class Attachment {
		/// <summary>
		/// Initializes a new instance of the <see cref="Attachment"/> class
		/// </summary>
		/// <param name="type"></param>
		protected Attachment(AttachmentType type) {
			Type = type;
		}

		/// <summary>
		/// Gets the type of the attachment.
		/// </summary>
		[JsonPropertyName("type")]
		public AttachmentType Type { get; }
	}
}
