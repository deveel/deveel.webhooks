namespace Deveel.Webhooks {
    public class WebhookHandlingOptions {
        /// <summary>
        /// Gets or sets the HTTP status code to return when the webhook
        /// processing is successful (<c>201</c> by default).
        /// </summary>
        public int? ResponseStatusCode { get; set; } = 204;

        /// <summary>
        /// Gets or sets the HTTP status code to return when the webhook
        /// processing failed for an internal error (<c>500</c> by default).
        /// </summary>
        public int? ErrorStatusCode { get; set; } = 500;

        /// <summary>
        /// Gets or sets the HTTP status code to return when the webhook
        /// from the sender is invalid (<c>400</c> by default).
        /// </summary>
        public int? InvalidStatusCode { get; set; } = 400;

        /// <summary>
        /// Gets or sets the execution mode for the handlers
        /// during the processing of a received webhook 
        /// (default: <see cref="HandlerExecutionMode.Parallel"/>).
        /// </summary>
        public HandlerExecutionMode? ExecutionMode { get; set; } = HandlerExecutionMode.Parallel;

        /// <summary>
        /// Gets or sets the maximum number of threads to use when
        /// executing the handlers in parallel. By default the number
        /// of processors in the machine is used.
        /// </summary>
        public int? MaxParallelThreads { get; set; }
    }
}
