using System;

using Deveel.Webhooks;

using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace Deveel {
	public static class ServiceCollectionExtensions {
		public static WebhookReceiverBuilder AddWebhooks<TWebhook>(this IServiceCollection services)
			where TWebhook : class {
			var builder= new WebhookReceiverBuilder(typeof(TWebhook), services);

			services.TryAddSingleton(builder);

			return builder;
		}

		public static IServiceCollection AddWebhooks<TWebhook>(this IServiceCollection services, Action<WebhookReceiverBuilder> configure) 
			where TWebhook : class {
			var builder = services.AddWebhooks<TWebhook>();
			configure?.Invoke(builder);

			return services;
		}
	}
}
