using System;

using Microsoft.Extensions.DependencyInjection;

namespace Deveel.Webhooks {
	public static class WebhookReceiverConfigurationBuilderExtensions {
		public static IWebhookReceiverConfigurationBuilder AddReceiver<TReceiver, TWebhook>(this IWebhookReceiverConfigurationBuilder builder, ServiceLifetime lifetime = ServiceLifetime.Singleton)
			where TReceiver : class, IWebhookReceiver<TWebhook>
			where TWebhook : class {
			return builder.ConfigureServices(services => {
				services.Add(new ServiceDescriptor(typeof(IWebhookReceiver<TWebhook>), typeof(TReceiver), lifetime));
				services.Add(new ServiceDescriptor(typeof(TReceiver), typeof(TReceiver), lifetime));
			});
		}

		public static IWebhookReceiverConfigurationBuilder AddReceiver<TWebhook>(this IWebhookReceiverConfigurationBuilder builder, ServiceLifetime lifetime = ServiceLifetime.Singleton)
			where TWebhook : class
			=> builder.AddReceiver<DefaultWebhookReceiver<TWebhook>, TWebhook>(lifetime);
	}
}
