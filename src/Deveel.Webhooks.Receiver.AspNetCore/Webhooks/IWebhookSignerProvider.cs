namespace Deveel.Webhooks {
	public interface IWebhookSignerProvider<TWebhook> {
		IWebhookSigner GetSigner(string algorithm);
	}
}
