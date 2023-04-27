using System.Text.Json.Serialization;

namespace Deveel.Facebook {
	public sealed class ImageAttachment : AssetAttachment {
		[JsonConstructor]
		public ImageAttachment(UrlPayload payload) 
			: base(AttachmentType.Image, payload) {
		}
	}
}
