using System.Text.Json.Serialization;

namespace Deveel.Facebook {
	/// <summary>
	/// Represents an attachment to a message that carries an asset.
	/// </summary>
	public class AssetAttachment : Attachment {
		/// <summary>
		/// Initializes the attachment with the given type and payload.
		/// </summary>
		/// <param name="type">The type of asset (either file, video, audio, video)</param>
		/// <param name="payload">The payload describing the references to
		/// the file asset</param>
		/// <exception cref="ArgumentException">
		/// Thrown if the given <paramref name="type"/> is a <see cref="AttachmentType.Template"/>,
		/// that is not a valid type for this type of attachment (see <see cref="TemplateAttachment"/>).
		/// </exception>
		/// <exception cref="ArgumentNullException">
		/// Thrown if the payload is <c>null</c>.
		/// </exception>
		[JsonConstructor]
		public AssetAttachment(AttachmentType type, UrlPayload payload) : base(type) {
			if (type == AttachmentType.Template)
				throw new ArgumentException("The template type is not valid", nameof(type));

			Payload = payload;
		}

		/// <summary>
		/// Gets the payload describing the references to the file asset.
		/// </summary>
		[JsonPropertyName("payload")]
		public UrlPayload Payload { get; }
	}
}
