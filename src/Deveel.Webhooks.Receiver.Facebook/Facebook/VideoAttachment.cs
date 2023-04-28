using System.Text.Json.Serialization;

namespace Deveel.Facebook {
	public sealed class VideoAttachment : AssetAttachment {
		[JsonConstructor]
		public VideoAttachment(UrlPayload payload) 
			: base(AttachmentType.Video, payload) {
		}
	}
}
