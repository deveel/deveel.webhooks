using System;

using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace Deveel.Webhooks {
	public static class ServiceCollectionExtensions {
		/// <summary>
		/// Adds the default services to support the webhook
		/// management provided by the framework.
		/// </summary>
		/// <param name="services">The collection of services</param>
		/// <param name="configure">A builder used to configure the service</param>
		/// <returns></returns>
		public static IServiceCollection AddWebhookSubscriptions<TSubscription>(this IServiceCollection services, Action<WebhookSubscriptionBuilder<TSubscription>> configure = null) 
			where TSubscription : class, IWebhookSubscription {

			var builder = services.AddWebhooksSubscriptions<TSubscription>();
			configure?.Invoke(builder);

			return services;
		}

		public static WebhookSubscriptionBuilder<TSubscription> AddWebhooksSubscriptions<TSubscription>(this IServiceCollection services) 
			where TSubscription : class, IWebhookSubscription {
			var builder = new WebhookSubscriptionBuilder<TSubscription>(services);

			services.TryAddSingleton(builder);

			return builder;
		}

	}
}