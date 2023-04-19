namespace Deveel.Webhooks {
    /// <summary>
    /// Represents the result of a verification of a webhook request.
    /// </summary>
    public readonly struct WebhookVerificationResult {
        /// <summary>
        /// Constructs the result of a verification of a webhook request.
        /// </summary>
        /// <param name="isVerified">
        /// Whether the request is verified or not
        /// </param>
        /// <param name="challenge">
        /// An optional response to be sent back to the sender of the webhook
        /// </param>
        public WebhookVerificationResult(bool isVerified, object? challenge = null) {
            IsVerified = isVerified;
            Challenge = challenge;
        }

        /// <summary>
        /// Gets whether the request is verified or not.
        /// </summary>
        public bool IsVerified { get; }

        /// <summary>
        /// Gets an optional response to be sent back to the sender of the webhook.
        /// </summary>
        public object? Challenge { get; }

        /// <summary>
        /// Creates a new result of a successful verification of a webhook request
        /// </summary>
        /// <param name="challenge">
        /// An optional response to be sent back to the sender of the webhook
        /// </param>
        /// <returns>
        /// Returns an instance of <see cref="WebhookVerificationResult"/> that
        /// represents a successful verification of a webhook request.
        /// </returns>
        public static WebhookVerificationResult Verified(object? challenge = null) => new WebhookVerificationResult(true, challenge);

        /// <summary>
        /// Creates a new result of a failed verification of a webhook request
        /// </summary>
        /// <returns>
        /// Returns a new instance of <see cref="WebhookVerificationResult"/> that
        /// represents a failed verification of a webhook request.
        /// </returns>
        public static WebhookVerificationResult Failed() => new WebhookVerificationResult(false);
    }
}
