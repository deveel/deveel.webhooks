namespace Deveel.Webhooks {
	public interface IWebhookCallback<TWebhook> {
		void OnWebhookHandled(TWebhook? webhook);
	}
}
