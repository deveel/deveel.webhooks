namespace Deveel.Webhooks {
	/// <summary>
	/// Provides a set of options for configuring a 
	/// Twilio webhook receiver.
	/// </summary>
	public sealed class TwilioReceiverOptions {
		/// <summary>
		/// Gets or sets the authentication token to use
		/// for the signature verification.
		/// </summary>
		public string? AuthToken { get; set; }

		/// <summary>
		/// Gets or sets the flag indicating if the signature
		/// of the webhook must be verified.
		/// </summary>
		public bool? VerifySignature { get; set; }
	}
}
