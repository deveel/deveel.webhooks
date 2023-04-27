using System.Text.Json.Serialization;

namespace Deveel.Facebook {
	/// <summary>
	/// Describes the metadata of an instance game play.
	/// </summary>
	public sealed class GameMetadata {
		/// <summary>
		/// Gets or sets the identifier of the player.
		/// </summary>
		[JsonPropertyName("player_id")]
		public string? PlayerId { get; set; }

		/// <summary>
		/// Gets or sets the identifier of the game context.
		/// </summary>
		[JsonPropertyName("context_id")]
		public string? ContextId { get; set; }
	}
}
