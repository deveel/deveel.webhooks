using System;

using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace Deveel.Webhooks {
	public sealed class WebhookSubscriptionBuilder<TSubscription> where TSubscription : class, IWebhookSubscription {
		public WebhookSubscriptionBuilder(IServiceCollection services) {
			Services = services ?? throw new ArgumentNullException(nameof(services));

			RegisterDefaults();
		}

		public IServiceCollection Services { get; }

		private void RegisterDefaults() {
			Services.TryAddScoped<WebhookSubscriptionManager<TSubscription>>();

			Services.TryAddScoped<IMultiTenantWebhookSubscriptionManager<TSubscription>, MultiTenantWebhookSubscriptionManager<TSubscription>>();
			Services.TryAddScoped<MultiTenantWebhookSubscriptionManager<TSubscription>>();

			Services.TryAddSingleton<IWebhookSubscriptionValidator<TSubscription>, WebhookSubscriptionValidator<TSubscription>>();
			Services.TryAddSingleton<IMultiTenantWebhookSubscriptionValidator<TSubscription>, MultiTenantWebhookSubscriptionValidator<TSubscription>>();

			Services.TryAddScoped<IWebhookSubscriptionResolver, DefaultWebhookSubscriptionResolver<TSubscription>>();
		}

		public WebhookSubscriptionBuilder<TSubscription> UseNotifier<TWebhook>(Action<WebhookNotifierBuilder<TWebhook>> configure)
			where TWebhook : class, IWebhook {
			Services.AddWebhookNotifier<TWebhook>(configure);

			return this;
		}

		public WebhookSubscriptionBuilder<TSubscription> UseManager<TManager>(ServiceLifetime lifetime = ServiceLifetime.Scoped)
			where TManager : WebhookSubscriptionManager<TSubscription> {

			Services.TryAdd(new ServiceDescriptor(typeof(WebhookSubscriptionManager<TSubscription>), typeof(TManager), lifetime));

			if (typeof(TManager) != typeof(WebhookSubscriptionManager<TSubscription>))
				Services.Add(new ServiceDescriptor(typeof(TManager), typeof(TManager), lifetime));

			return this;
		}

		public WebhookSubscriptionBuilder<TSubscription> UseManager()
			=> UseManager<WebhookSubscriptionManager<TSubscription>>();


		public WebhookSubscriptionBuilder<TSubscription> AddValidator<TValidator>(ServiceLifetime lifetime = ServiceLifetime.Singleton)
			where TValidator : class, IWebhookSubscriptionValidator<TSubscription> {
			Services.Add(new ServiceDescriptor(typeof(IWebhookSubscriptionValidator<TSubscription>), typeof(TValidator), lifetime));
			Services.Add(new ServiceDescriptor(typeof(TValidator), typeof(TValidator), lifetime));

			return this;
		}
	}
}
