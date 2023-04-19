using Deveel.Webhooks.Model;

using Newtonsoft.Json;

namespace Deveel.Webhooks.Handlers {
	public class TestSignedWebhookHandler : IWebhookHandler<TestSignedWebhook> {
		private readonly IWebhookCallback<TestSignedWebhook> _callback;

		public TestSignedWebhookHandler(IWebhookCallback<TestSignedWebhook> callback) {
			_callback = callback;
		}

		public Task HandleAsync(TestSignedWebhook webhook, CancellationToken cancellationToken = default) {
			_callback.OnWebhookHandled(webhook);

			return Task.CompletedTask;
		}
	}
}
