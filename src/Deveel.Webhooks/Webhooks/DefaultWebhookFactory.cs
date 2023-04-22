namespace Deveel.Webhooks {
	/// <summary>
	/// A default implementation of the <see cref="IWebhookFactory{TWebhook}"/>
	/// that creates a <see cref="Webhook"/> instance using the information
	/// provided by the subscription and the event.
	/// </summary>
	public sealed class DefaultWebhookFactory : DefaultWebhookFactory<Webhook> {
	}
}
