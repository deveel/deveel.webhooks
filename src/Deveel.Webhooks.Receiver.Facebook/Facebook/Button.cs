using System.Text.Json.Serialization;

using Deveel.Json;

namespace Deveel.Facebook {
	/// <summary>
	/// Represents a button that can be added to a message.
	/// </summary>
	[JsonConverter(typeof(PolymorphicJsonConverter<Button>))]
	[JsonDiscriminator("type")]
	[JsonKnownType(typeof(WebUrlButton), ButtonTypeNames.WebUrl)]
	[JsonKnownType(typeof(PostbackButton), ButtonTypeNames.Postback)]
	[JsonKnownType(typeof(CallButton), ButtonTypeNames.PhoneNumber)]
	[JsonKnownType(typeof(GamePlayButton), ButtonTypeNames.GamePlay)]
	public abstract class Button {
		/// <summary>
		/// Initializes a new instance of the <see cref="Button"/> class.
		/// </summary>
		/// <param name="type">The type of button</param>
		protected Button(ButtonType type) {
			Type = type;
		}

		/// <summary>
		/// Gets the type of button.
		/// </summary>
		[JsonPropertyName("type")]
		public ButtonType Type { get; }
	}
}
