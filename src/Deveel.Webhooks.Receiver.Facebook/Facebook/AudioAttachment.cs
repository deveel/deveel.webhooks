using System.Text.Json.Serialization;

namespace Deveel.Facebook {
	/// <summary>
	/// Represents an audio file attachment.
	/// </summary>
	public sealed class AudioAttachment : AssetAttachment {
		/// <summary>
		/// Initializes a new instance of the <see cref="AudioAttachment"/> class.
		/// </summary>
		/// <param name="payload">The payload that describes the file references</param>
		[JsonConstructor]
		public AudioAttachment(UrlPayload payload) 
			: base(AttachmentType.Audio, payload) {
		}
	}
}
