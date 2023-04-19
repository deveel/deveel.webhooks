using System;

namespace Deveel.Webhooks {
    /// <summary>
    /// Provides a set of options to configure the behavior of the
    /// verification process of a webhook request.
    /// </summary>
    public class WebhookVerificationOptions {
        /// <summary>
        /// Gets or sets the HTTP status code to return when the request
        /// is authorized (<c>200</c> by default).
        /// </summary>
        public int? SuccessStatusCode { get; set; } = 200;

        /// <summary>
        /// Gets or sets the HTTP status code to return when the request
        /// fails due to an internal error (<c>500</c> by default).
        /// </summary>
        public int? FailureStatusCode { get; set; } = 500;

        /// <summary>
        /// Gets or sets the HTTP status code to return when the request
        /// is not authorized (<c>403</c> by default).
        /// </summary>
        public int? NotAuthorizedStatusCode { get; set; } = 403;
    }
}
