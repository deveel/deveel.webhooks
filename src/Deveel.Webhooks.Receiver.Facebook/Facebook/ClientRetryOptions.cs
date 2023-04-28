using System;

namespace Deveel.Facebook {
    /// <summary>
    /// Provides options to configure the retry policy of the client.
    /// </summary>
    public sealed class ClientRetryOptions {
        /// <summary>
        /// Gets or sets the maximum number of retries to perform
        /// (by default set to <c>3</c>).
        /// </summary>
        public int MaxRetries { get; set; } = 3;

        /// <summary>
        /// The maximum amount in milliseconds to wait before 
        /// retrying a request (by default <c>500</c>).
        /// </summary>
        public int MaxDelay { get; set; } = 500;
    }
}
