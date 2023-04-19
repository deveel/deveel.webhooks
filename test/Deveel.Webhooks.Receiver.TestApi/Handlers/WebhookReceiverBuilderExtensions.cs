namespace Deveel.Webhooks.Handlers {
	public static class WebhookReceiverBuilderExtensions {
		public static WebhookReceiverBuilder<TWebhook> UseCallback<TWebhook>(this WebhookReceiverBuilder<TWebhook> builder, IWebhookCallback<TWebhook> callback) 
			where TWebhook : class {
			builder.Services.AddSingleton(callback);
			return builder;
		}

		public static WebhookReceiverBuilder<TWebhook> UseCallback<TWebhook>(this WebhookReceiverBuilder<TWebhook> builder, Action<TWebhook?> callback) 
			where TWebhook : class {
			builder.Services.AddSingleton<IWebhookCallback<TWebhook>>(new DelegatedWebhookCallback<TWebhook>(callback));
			return builder;
		}
	}

	class DelegatedWebhookCallback<TWebhook> : IWebhookCallback<TWebhook> where TWebhook : class {
		private readonly Action<TWebhook?> callback;

		public DelegatedWebhookCallback(Action<TWebhook?> callback) {
			this.callback = callback;
		}

		public void OnWebhookHandled(TWebhook? webhook) => callback(webhook);
	}
}
