namespace Deveel.Webhooks {
    /// <summary>
    /// An exception thrown when the verification of a webhook destination fails.
    /// </summary>
    public class WebhookVerificationException : WebhookSenderException {
        /// <inheritdoc/>
        public WebhookVerificationException() {
        }

        /// <inheritdoc/>
        public WebhookVerificationException(string? message) : base(message) {
        }

        /// <inheritdoc/>
        public WebhookVerificationException(string? message, Exception? innerException) : base(message, innerException) {
        }
    }
}
