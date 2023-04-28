// Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
#pragma warning disable CS8618

namespace Deveel.Webhooks.Twilio {
	/// <summary>
	/// Represents a part of a messaging conversation.
	/// </summary>
    public sealed record class MessagePart {
		/// <summary>
		/// Gets the phone number of the messaging part
		/// </summary>
		public string PhoneNumber { get; internal set; }

		/// <summary>
		/// Gets the country code of the messaging part
		/// </summary>
		public string? Country { get; internal set; }

		/// <summary>
		/// Gets the state of the messaging part
		/// </summary>
        public string? State { get; internal set; }

		/// <summary>
		/// Gets the city name of the messaging part
		/// </summary>
        public string? City { get; internal set; }

		/// <summary>
		/// Gets the postal code of the messaging part
		/// </summary>
        public string? Zip { get; internal set; }
    }
}
