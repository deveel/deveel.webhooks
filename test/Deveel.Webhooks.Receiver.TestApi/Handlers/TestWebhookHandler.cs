using Deveel.Webhooks.Model;

using Microsoft.Extensions.Options;

namespace Deveel.Webhooks.Handlers {
	public class TestWebhookHandler : IWebhookHandler<TestWebhook> {
		private readonly WebhookReceiverOptions options;
		private readonly IWebhookCallback<TestWebhook> callback;

		public TestWebhookHandler(IOptionsSnapshot<WebhookReceiverOptions> options, IWebhookCallback<TestWebhook> callback) {
			this.options = options.GetReceiverOptions<TestWebhook>();
			this.callback = callback;
		}

		public Task HandleAsync(TestWebhook webhook, CancellationToken cancellationToken = default) {
			callback.OnWebhookHandled(webhook);
			return Task.CompletedTask;
		}
	}
}
