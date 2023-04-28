// Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
#pragma warning disable CS8618


namespace Deveel.Webhooks.Twilio {
	/// <summary>
	/// Represents a message webhook from Twilio
	/// </summary>
    public sealed class TwilioWebhook {
		/// <summary>
		/// Gets the <see cref="MessagePart"/> that represents the sender 
		/// of the message.
		/// </summary>
        public MessagePart From { get; } = new MessagePart();

		/// <summary>
		/// Gets the <see cref="MessagePart"/> that represents the receiver
		/// of the message.
		/// </summary>
        public MessagePart To { get; } = new MessagePart();

		/// <summary>
		/// Gets the unique ID of the SMS message sent to the recipient's phone
		/// </summary>
		public string SmsId { get; internal set; }

		/// <summary>
		/// Gets the status of the message
		/// </summary>
		public MessageStatus MessageStatus { get; internal set; }

		/// <summary>
		/// Gets the body of the message
		/// </summary>
        public string? Body { get; internal set; }

		/// <summary>
		/// Gets the number of segments used to send the message
		/// </summary>
		public int? SegmentCount { get; internal set; }

		/// <summary>
		/// Gets the unique identifier of the message
		/// </summary>
        public string MessageId { get; internal set; }

		/// <summary>
		/// Gets the unique identifier of the Account that 
		/// initiated the message
		/// </summary>
		public string AccountId { get; internal set; }

		/// <summary>
		/// Gets the version of the Twilio API used to handle the message
		/// </summary>
		public string? ApiVersion { get; internal set; }

		/// <summary>
		/// Gets an optional array of media elements that are attached to 
		/// the message, referencing external resources.
		/// </summary>
		public MediaElement[]? Media { get; internal set; }

		/// <summary>
		/// Gets an optional array of segments that compose the message,
		/// when the message is longer than 160 characters.
		/// </summary>
		public Segment[]? Segments { get; internal set; }

		/// <summary>
		/// Gets the error code, if the message was not delivered
		/// </summary>
		public string? ErrorCode { get; internal set; }

		/// <summary>
		/// Gets the error message, if the message was not delivered
		/// </summary>
		public string? ErrorMessage { get; internal set; }
    }
}
