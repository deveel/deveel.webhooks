namespace Deveel.Webhooks {
	/// <summary>
	/// The default configuration values for the webhooks sender.
	/// </summary>
	public static class WebhookSenderDefaults {
		/// <summary>
		/// Whether the sender should add trace headers to the 
		/// HTTP request (default: <c>true</c>).
		/// </summary>
		public const bool AddTraceHeaders = true;

		/// <summary>
		/// The name of the header that will be used to send the
		/// trace ID (default: <c>X-Webhook-TraceId</c>).
		/// </summary>
		public const string TraceHeaderName = "X-Webhook-TraceId";

		/// <summary>
		/// The name of the header that will be used to send the
		/// attempt number of the webhook (default: <c>X-Webhook-Attempt</c>).
		/// </summary>
		public const string AttemptTraceHeaderName = "X-Webhook-Attempt";

		/// <summary>
		/// The default location of the signature in the HTTP request
		/// (default: <see cref="WebhookSignatureLocation.Header"/>).
		/// </summary>
		public const WebhookSignatureLocation SignatureLocation = WebhookSignatureLocation.Header;

		/// <summary>
		/// Gets the default value for the flag indicating if the
		/// webhook request should be signed or not (default: <c>true</c>).
		/// </summary>
		public const bool SignWebhooks = true;

		/// <summary>
		/// The default name of the header that will be used to send
		/// the signature of the webhook (default: <c>X-Webhook-Signature</c>).
		/// </summary>
		public const string SignatureHeaderName = "X-Webhook-Signature";

		/// <summary>
		/// The default name of the query parameter that will be used
		/// to send the signature of the webhook (default: <c>signature</c>).
		/// </summary>
		public const string SignatureQueryParameterName = "signature";

		/// <summary>
		/// The default format of the webhook payload (default: <see cref="WebhookFormat.Json"/>).
		/// </summary>
		public const WebhookFormat Format = WebhookFormat.Json;

		/// <summary>
		/// The default content type for the JSON payload (default: <c>application/json</c>).
		/// </summary>
		public const string JsonContentType = "application/json";

		/// <summary>
		/// The default content type for the XML payload (default: <c>text/xml</c>).
		/// </summary>
		public const string XmlContentType = "text/xml";

		/// <summary>
		/// The default header name for the verification token (default: <c>X-Verify-Token</c>).
		/// </summary>
		public const string VerifyTokenHeaderName = "X-Verify-Token";

		/// <summary>
		/// The default query parameter name for the verification token (default: <c>token</c>).
		/// </summary>
		public const string VerifyTokenQueryParameterName = "token";

		/// <summary>
		/// The default HTTP method to use for the verification request (default: <c>GET</c>).
		/// </summary>
		public const string VerifyHttpMethod = "GET";

		/// <summary>
		/// The default value for the flag indicating if the sender should
		/// challenge the receiver of the webhook during the verification
		/// process (default: <c>false</c>).
		/// </summary>
		public const bool VerificationChallenge = false;

		/// <summary>
		/// If the verification challenge is enabled, this is the default name
		/// of the query parameter that will be used to send the challenge
		/// to the receiver (default: <c>challenge</c>).
		/// </summary>
		public const string ChallengeQueryParameterName = "challenge";

		/// <summary>
		/// The default location of the verification token in the HTTP request
		/// (default: <see cref="VerificationTokenLocation.QueryString"/>).
		/// </summary>
		public const VerificationTokenLocation DefaultVerificationTokenLocation = VerificationTokenLocation.QueryString;
	}
}
