using Deveel.Webhooks.Model;

using Microsoft.Extensions.Options;

namespace Deveel.Webhooks.Handlers {
	public class TestWebhookHandler : IWebhookHandler<TestWebhook> {
		private readonly WebhookReceiverOptions<TestWebhook> options;
		private readonly IWebhookCallback<TestWebhook> callback;

		public TestWebhookHandler(IOptions<WebhookReceiverOptions<TestWebhook>> options, IWebhookCallback<TestWebhook> callback) {
			this.options = options.Value;
			this.callback = callback;
		}

		public Task HandleAsync(TestWebhook webhook, CancellationToken cancellationToken = default) {
			callback.OnWebhookHandled(webhook);
			return Task.CompletedTask;
		}
	}
}
