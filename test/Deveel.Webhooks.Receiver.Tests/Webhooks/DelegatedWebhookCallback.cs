namespace Deveel.Webhooks {
	class DelegatedWebhookCallback<TWebhook> : IWebhookCallback<TWebhook> where TWebhook : class {
		private readonly Action<TWebhook?> callback;

		public DelegatedWebhookCallback(Action<TWebhook?> callback) {
			this.callback = callback;
		}

		public void OnWebhookHandled(TWebhook? webhook) => callback(webhook);
	}
}
