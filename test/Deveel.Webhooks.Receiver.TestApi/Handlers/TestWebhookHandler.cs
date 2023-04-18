using Deveel.Webhooks.Model;

using Microsoft.Extensions.Options;

using Newtonsoft.Json;

namespace Deveel.Webhooks.Handlers {
	public class TestWebhookHandler : IWebhookHandler<TestWebhook> {
		private readonly ILogger<TestWebhookHandler> _logger;
		private readonly WebhookReceiverOptions<TestWebhook> options;

		public TestWebhookHandler(IOptions<WebhookReceiverOptions<TestWebhook>> options, ILogger<TestWebhookHandler> logger) {
			_logger = logger;
			this.options = options.Value;
		}

		public Task HandleAsync(TestWebhook webhook, CancellationToken cancellationToken = default) {
			_logger.LogInformation(JsonConvert.SerializeObject(webhook));

			return Task.CompletedTask;
		}
	}
}
