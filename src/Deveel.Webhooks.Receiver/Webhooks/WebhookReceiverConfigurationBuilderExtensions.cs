using System;

using Deveel.Webhooks;

using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

using Newtonsoft.Json.Linq;

namespace Deveel.Webhooks {
	public static class WebhookReceiverConfigurationBuilderExtensions {
		public static IWebhookReceiverConfigurationBuilder Configure(this IWebhookReceiverConfigurationBuilder builder, Action<WebhookReceiveOptions> configure) {
			if (configure != null)
				builder.ConfigureServices(services => services.Configure(configure));

			return builder;
		}

		public static IWebhookReceiverConfigurationBuilder AddWebhookOptions(this IWebhookReceiverConfigurationBuilder builder, WebhookReceiveOptions options) {
			return builder.ConfigureServices(services => services.AddSingleton<WebhookReceiveOptions>(options));
		}

		public static IWebhookReceiverConfigurationBuilder AddHttpReceiver<TReceiver, TWebhook>(this IWebhookReceiverConfigurationBuilder builder, ServiceLifetime lifetime = ServiceLifetime.Scoped)
			where TReceiver : class, IHttpWebhookReceiver<TWebhook>
			where TWebhook : class {
			builder.ConfigureServices(services => {
				services.Add(new ServiceDescriptor(typeof(IHttpWebhookReceiver<TWebhook>), typeof(TReceiver), lifetime));
				services.Add(new ServiceDescriptor(typeof(TReceiver), lifetime));
			});

			return builder;
		}

		public static IWebhookReceiverConfigurationBuilder AddHttpReceiver<TReceiver, TWebhook>(this IWebhookReceiverConfigurationBuilder builder, TReceiver receiver)
			where TReceiver : class, IHttpWebhookReceiver<TWebhook>
			where TWebhook : class {
			builder.ConfigureServices(services => services
				.AddSingleton<IHttpWebhookReceiver<TWebhook>>(receiver)
				.AddSingleton(receiver));
			return builder;
		}

		public static IWebhookReceiverConfigurationBuilder AddHttpReceiver<T>(this IWebhookReceiverConfigurationBuilder builder)
			where T : class
			=> builder.AddHttpReceiver<DefaultHttptWebhookReceiver<T>, T>();

	}
}
