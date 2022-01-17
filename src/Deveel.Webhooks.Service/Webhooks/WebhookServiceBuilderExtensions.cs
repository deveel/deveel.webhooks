using System;
using System.Collections.Generic;
using System.Diagnostics;

using Deveel.Webhooks;
using Deveel.Webhooks.Storage;

using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace Deveel.Webhooks {
	public static class WebhookServiceBuilderExtensions {		
		public static IWebhookServiceBuilder UseDefaultSubscriptionResolver(this IWebhookServiceBuilder builder)
			=> builder.UseSubscriptionResolver<DefaultWebhookSubscriptionResolver>();

		public static IWebhookServiceBuilder UseSubscriptionManager<TManager>(this IWebhookServiceBuilder builder)
			where TManager : class, IWebhookSubscriptionManager
			=> builder.Use<IWebhookSubscriptionManager, TManager>();

		public static IWebhookServiceBuilder UseSubscriptionManager(this IWebhookServiceBuilder builder) {
			builder.ConfigureServices(services => {
				services.TryAddScoped<IWebhookSubscriptionResolver, DefaultWebhookSubscriptionResolver>();
			});

			return builder.UseSubscriptionManager<DefaultWebhookSubscriptionManager>();
		}
	}
}
