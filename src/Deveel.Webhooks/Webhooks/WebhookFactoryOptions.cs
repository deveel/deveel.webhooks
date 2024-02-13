namespace Deveel.Webhooks {
	/// <summary>
	/// Configures the behavior of the <see cref="DefaultWebhookFactory{TWebhook}"/>
	/// when creating webhooks from a notification for a subscription.
	/// </summary>
	/// <typeparam name="TWebhook"></typeparam>
	public class WebhookFactoryOptions<TWebhook> {
		/// <summary>
		/// Gets or sets the strategy to use when creating webhooks
		/// from a notification for a subscription.
		/// </summary>
		public WebhookCreateStrategy CreateStrategy { get; set; } = WebhookCreateStrategy.OnePerNotification;
	}
}
