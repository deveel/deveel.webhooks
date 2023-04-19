namespace Deveel.Webhooks.Handlers {
	public interface IWebhookCallback<TWebhook> {
		void OnWebhookHandled(TWebhook? webhook);
	}
}
