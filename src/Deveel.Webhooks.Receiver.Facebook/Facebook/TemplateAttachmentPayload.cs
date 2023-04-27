using System.Text.Json.Serialization;

using Deveel.Json;

namespace Deveel.Facebook {
	/// <summary>
	/// Describes the payload of a templated attachment.
	/// </summary>
	//[JsonConverter(typeof(PolymorphicJsonConverter<TemplateAttachmentPayload>))]
	//[JsonDiscriminator("template_type")]
	//[JsonKnownType(typeof(GenericTemplate), "generic")]
	//[JsonKnownType(typeof(ButtonTemplate), "button")]
	//[JsonKnownType(typeof(MediaTemplate), "media")]
	//[JsonKnownType(typeof(ReceiptTemplate), "receipt")]
	//[JsonKnownType(typeof(ProductTemplate), "product")]
	public abstract class TemplateAttachmentPayload {
		///// <summary>
		///// Initializes a new instance of the <see cref="TemplateAttachmentPayload"/> class.
		///// </summary>
		///// <param name="type">The type of templated message</param>
		//protected TemplateAttachmentPayload(TemplateType type) {
		//	Type = type;
		//}

		///// <summary>
		///// Gets the type of templated message.
		///// </summary>
		//[JsonPropertyName("template_type")]
		//public TemplateType Type { get; }
	}
}
