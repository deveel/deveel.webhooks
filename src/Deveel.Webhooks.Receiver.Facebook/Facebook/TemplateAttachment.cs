using System.Text.Json.Serialization;

namespace Deveel.Facebook {
	public sealed class TemplateAttachment : Attachment {
		[JsonConstructor]
		public TemplateAttachment(TemplateAttachmentPayload payload) : base(AttachmentType.Template) {
			Payload = payload ?? throw new ArgumentNullException(nameof(payload));
		}

		[JsonPropertyName("payload")]
		public TemplateAttachmentPayload Payload { get; }
	}
}
