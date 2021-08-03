using System;

using Deveel.Webhooks;

using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

using Newtonsoft.Json.Linq;

namespace Deveel.Webhooks {
	public static class WebhookReceiverConfigurationBuilderExtensions {
		public static IWebhookReceiverConfigurationBuilder Configure(this IWebhookReceiverConfigurationBuilder builder, Action<WebhookReceiveOptions> configure) {
			if (configure != null)
				builder.Services.Configure(configure);

			return builder;
		}

		public static IWebhookReceiverConfigurationBuilder AddReceiver<TReceiver, TWebhook>(this IWebhookReceiverConfigurationBuilder builder)
			where TReceiver : class, IWebhookReceiver<TWebhook>
			where TWebhook : class {
			builder.Services.AddScoped<IWebhookReceiver<TWebhook>, TReceiver>();
			return builder;
		}

		public static IWebhookReceiverConfigurationBuilder AddReceiver<TReceiver, TWebhook>(this IWebhookReceiverConfigurationBuilder builder, TReceiver receiver)
			where TReceiver : class, IWebhookReceiver<TWebhook>
			where TWebhook : class {
			builder.Services.AddSingleton<IWebhookReceiver<TWebhook>>(receiver);
			return builder;
		}

		public static IWebhookReceiverConfigurationBuilder AddReceiver<T>(this IWebhookReceiverConfigurationBuilder builder)
			where T : class
			=> builder.AddReceiver<DefaultWebhookReceiver<T>, T>();

		public static IWebhookReceiverConfigurationBuilder AddReceiver<T>(this IWebhookReceiverConfigurationBuilder builder, Action<JObject, T> afterRead)
			where T : class {
			builder.Services.AddScoped<IWebhookReceiver<T>>(provider =>
				new DefaultWebhookReceiver<T>(provider.GetService<IOptions<WebhookReceiveOptions>>(), afterRead));
			return builder;
		}

	}
}
