using System.Text.Json.Serialization;

namespace Deveel.Facebook {
	/// <summary>
	/// Represents a generic file attachment.
	/// </summary>
	public sealed class FileAttachment : AssetAttachment {
		/// <summary>
		/// Initializes the attachment with the given payload.
		/// </summary>
		/// <param name="payload"></param>
		[JsonConstructor]
		public FileAttachment(UrlPayload payload) 
			: base(AttachmentType.File, payload) {
		}
	}
}
