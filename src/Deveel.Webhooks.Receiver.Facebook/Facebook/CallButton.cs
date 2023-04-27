using System.Text.Json.Serialization;

namespace Deveel.Facebook {
	/// <summary>
	/// Represents a button that triggers a call to a phone number.
	/// </summary>
	public sealed class CallButton : Button {
        /// <summary>
        /// Initializes a new instance of the <see cref="CallButton"/> class
        /// with the given phone number and label.
        /// </summary>
        /// <param name="phoneNumber">The phone number to be called, that
        /// must be in the international form <c>+[COUNTRY_CODE][PHONE_NUMBER]</c> (eg. +15105559999)</param>
        /// <param name="title">A label that is used to mask the phone number</param>
        /// <exception cref="ArgumentException">
		/// Thrown if the given <paramref name="phoneNumber"/> or <paramref name="title"/> is <c>null</c> or empty.
        /// </exception>
        [JsonConstructor]
		public CallButton(string phoneNumber, string title)
			: base(ButtonType.PhoneNumber) {
			if (string.IsNullOrWhiteSpace(phoneNumber))
				throw new ArgumentException($"'{nameof(phoneNumber)}' cannot be null or whitespace.", nameof(phoneNumber));
            if (String.IsNullOrWhiteSpace(title))
                throw new ArgumentException($"'{nameof(title)}' cannot be null or whitespace.", nameof(title));

            PhoneNumber = phoneNumber;
			Title = title;
		}

		/// <summary>
		/// Gets the phone number to be called.
		/// </summary>
		[JsonPropertyName("payload")]
		public string PhoneNumber { get; }

		/// <summary>
		/// Gets the label that is used to mask the phone number.
		/// </summary>
		[JsonPropertyName("title")]
		public string Title { get; }
	}
}
