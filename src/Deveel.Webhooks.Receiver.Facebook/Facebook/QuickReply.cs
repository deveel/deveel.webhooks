using System;
using System.Text.Json.Serialization;

using Deveel.Json;

namespace Deveel.Facebook {
	[JsonConverter(typeof(PolymorphicJsonConverter<QuickReply>))]
	[JsonDiscriminator("content_type")]
	[JsonKnownType(typeof(TextQuickReply), "text")]
	[JsonKnownType(typeof(UserPhoneQuickReply), "user_phone_number")]
	[JsonKnownType(typeof(UserEmailQuickReply), "user_email")]
	public abstract class QuickReply {
		protected QuickReply(QuickReplyType contentType) {
			ContentType = contentType;
		}

		[JsonPropertyName("content_type")]
		public QuickReplyType ContentType { get; }
	}
}
