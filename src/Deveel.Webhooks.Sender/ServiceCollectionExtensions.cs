using System;

using Deveel.Webhooks;

using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace Deveel {
	public static class ServiceCollectionExtensions {
		public static WebhookSenderBuilder<TWebhook> AddWebhookSender<TWebhook>(this IServiceCollection services)
			where TWebhook : class {
			var builder = new WebhookSenderBuilder<TWebhook>(services);

			services.TryAddSingleton(builder);

			return builder;
		}

		public static IServiceCollection AddWebhookSender<TWebhook>(this IServiceCollection services, Action<WebhookSenderBuilder<TWebhook>> configure)
			where TWebhook : class {
			var builder = services.AddWebhookSender<TWebhook>();
			configure?.Invoke(builder);

			return services;
		}
	}
}
