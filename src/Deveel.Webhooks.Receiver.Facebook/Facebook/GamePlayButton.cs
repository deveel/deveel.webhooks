using System.Text.Json.Serialization;

namespace Deveel.Facebook {
	/// <summary>
	/// Represents a button that launches an Instant Game that is associated
	/// with a Facebook Page.
	/// </summary>
	public sealed class GamePlayButton : Button {
		/// <summary>
		/// Initializes a new instance of the <see cref="GamePlayButton"/> class with the
		/// given title for the button.
		/// </summary>
		/// <param name="title">The title that masks the button</param>
		[JsonConstructor]
		public GamePlayButton(string title) : base(ButtonType.GamePlay) {
            if (String.IsNullOrWhiteSpace(title))
                throw new ArgumentException($"'{nameof(title)}' cannot be null or whitespace.", nameof(title));

            Title = title;
		}

		/// <summary>
		/// Gets the title that masks the button.
		/// </summary>
		[JsonPropertyName("title")]
		public string Title { get; }

		/// <summary>
		/// Gets or sets the payload to be sent to the game.
		/// </summary>
		[JsonPropertyName("payload")]
		public string? Payload { get; set; }

		/// <summary>
		/// Gets or sets an optional set of metadata that defines
		/// the start if the game.
		/// </summary>
		[JsonPropertyName("game_metadata")]
		public GameMetadata? GameMetadata { get; set; }
	}
}
