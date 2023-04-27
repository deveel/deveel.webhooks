using Microsoft.Extensions.DependencyInjection;

namespace Deveel.Webhooks {
	public static class WebhookReceiverBuilderExtensions {
		public static WebhookReceiverBuilder<TWebhook> UseCallback<TWebhook>(this WebhookReceiverBuilder<TWebhook> builder, IWebhookCallback<TWebhook> callback) 
			where TWebhook : class {
			builder.Services.AddCallback(callback);
			return builder;
		}

		public static WebhookReceiverBuilder<TWebhook> UseCallback<TWebhook>(this WebhookReceiverBuilder<TWebhook> builder, Action<TWebhook?> callback) 
			where TWebhook : class {
			builder.Services.AddCallback(callback);
			return builder;
		}
	}
}
