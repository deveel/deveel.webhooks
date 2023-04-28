using System;
using System.Text.Json.Serialization;

namespace Deveel.Facebook {
	public sealed class Message {
		[JsonPropertyName("mid")]
		public string Id { get; set; }

		[JsonPropertyName("reply_to")]
		public MessageRef? ReplyTo { get; set; }

		[JsonPropertyName("text")]
		public string? Text { get; set; }

		[JsonPropertyName("attachments")]
		public Attachment[] Attachments { get; set; }

		[JsonPropertyName("quick_reply")]
		public QuickReply? QuickReply { get; set; }
	}
}
