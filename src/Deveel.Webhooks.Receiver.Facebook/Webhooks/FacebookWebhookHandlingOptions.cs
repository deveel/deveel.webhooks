namespace Deveel.Webhooks {
	public sealed class FacebookWebhookHandlingOptions {
		public int MaxParallelThreads { get; set; } = Environment.ProcessorCount - 1;

		public HandlerExecutionMode ExecutionMode { get; set; } = HandlerExecutionMode.Parallel;
	}
}
