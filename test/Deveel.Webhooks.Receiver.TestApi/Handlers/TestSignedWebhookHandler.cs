using Deveel.Webhooks.Model;

using Newtonsoft.Json;

namespace Deveel.Webhooks.Handlers {
	public class TestSignedWebhookHandler : IWebhookHandler<TestSignedWebhook> {
		private readonly ILogger<TestSignedWebhookHandler> _logger;

		public TestSignedWebhookHandler(ILogger<TestSignedWebhookHandler> logger) {
			_logger = logger;
		}

		public Task HandleAsync(TestSignedWebhook webhook, CancellationToken cancellationToken = default) {
			_logger.LogInformation(JsonConvert.SerializeObject(webhook));

			return Task.CompletedTask;
		}
	}
}
