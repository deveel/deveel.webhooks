using System.Text.Json.Serialization;

namespace Deveel.Facebook {
    /// <summary>
    /// Represents a button that launches the navigation to a given URL.
    /// </summary>
    public sealed class WebUrlButton : Button {
		/// <summary>
		/// Initializes a new instance of the <see cref="WebUrlButton"/> class with the
		/// given URL and title for the button.
		/// </summary>
		/// <param name="url">The web URL for the navigation</param>
		/// <param name="title">The title of the button used to mask the navigation URL</param>
		/// <exception cref="ArgumentException">
		/// Thrown if the <paramref name="url"/> or <paramref name="title"/> is <c>null</c> or empty.
		/// </exception>
		[JsonConstructor]
		public WebUrlButton(string url, string title)
			: base(ButtonType.WebUrl) {
			if (string.IsNullOrWhiteSpace(url))
				throw new ArgumentException($"'{nameof(url)}' cannot be null or whitespace.", nameof(url));
            if (String.IsNullOrWhiteSpace(title))
                throw new ArgumentException($"'{nameof(title)}' cannot be null or whitespace.", nameof(title));

            Url = url;
			Title = title;
		}

		/// <summary>
		/// Gets the title of the button used to mask the navigation URL.
		/// </summary>
		[JsonPropertyName("title")]
		public string Title { get; }

		/// <summary>
		/// Gets the web URL for the navigation.
		/// </summary>
		[JsonPropertyName("url")]
		public string Url { get; }
	}
}
