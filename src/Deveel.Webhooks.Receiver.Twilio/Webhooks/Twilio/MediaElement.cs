namespace Deveel.Webhooks.Twilio {
	/// <summary>
	/// Represents the reference to a media element in a Twilio webhook.
	/// </summary>
	public sealed class MediaElement {
		/// <summary>
		/// Gets the URL of the media element.
		/// </summary>
        public string Url { get; internal set; }

		/// <summary>
		/// Gets the content type of the media element.
		/// </summary>
		public string? ContentType { get; internal set; }

		/// <summary>
		/// Gets the size of the media element.
		/// </summary>
		public long? ContentLength { get; internal set; }
	}
}
