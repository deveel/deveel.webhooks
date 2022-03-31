using System;
using System.Collections.Generic;
using System.Text;

using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

using Polly;

namespace Deveel.Webhooks {
	public sealed class WebhookServiceBuilder<TSubscription> where TSubscription : class, IWebhookSubscription {
		internal WebhookServiceBuilder(IServiceCollection services) {
			Services = services;
		}

		public IServiceCollection Services { get; }

		internal void AddDefaults() {
			Services.TryAddScoped<IWebhookNotifier, WebhookNotifier>();
			Services.AddScoped<WebhookNotifier>();

			Services.TryAddScoped<IWebhookSender, WebhookSender>();
			Services.AddScoped<WebhookSender>();

			Services.AddSingleton<IWebhookSigner, Sha256WebhookSigner>();
			Services.AddSingleton<Sha256WebhookSigner>();

			Services.AddSingleton<IWebhookSerializer, JsonWebhookSerializer>();
			Services.AddSingleton<JsonWebhookSerializer>();

			Services.AddScoped<IWebhookServiceConfiguration, WebhookServiceConfiguration>();

			Services.AddOptions<WebhookDeliveryOptions>();

		}

		public WebhookServiceBuilder<TSubscription> ConfigureDelivery(Action<WebhookDeliveryOptions> configure) {
			if (configure != null)
				Services.AddOptions<WebhookDeliveryOptions>().Configure(configure);

			return this;
		}

		public WebhookServiceBuilder<TSubscription> ConfigureDelivery(WebhookDeliveryOptions options) {
			Services.AddSingleton(options);
			return this;
		}

		public WebhookServiceBuilder<TSubscription>  ConfigureDelivery(string sectionName) {
			Services.AddOptions<WebhookDeliveryOptions>()
				.Configure<IConfiguration>((options, config) => {
					var section = config.GetSection(sectionName);
					if (section != null)
						section.Bind(options);
				});

			return this;
		}

		public WebhookServiceBuilder<TSubscription> ConfigureDelivery(Action<IWebhookDeliveryOptionsBuilder> options) {
			Services
				.AddOptions<WebhookDeliveryOptions>()
				.Configure(opts => {
					var optionsBuilder = new WebhookDeliveryOptionsBuilder(opts);
					options(optionsBuilder);
				});

			return this;
		}

		public WebhookServiceBuilder<TSubscription> ConfigureDefaultDelivery() {
			Services.AddOptions<WebhookDeliveryOptions>();
			return this;
		}

		/// <summary>
		/// Replaces the notifier service with another one.
		/// </summary>
		/// <typeparam name="TNotifier">The type of the new <see cref="IWebhookNotifier"/> to be used</typeparam>
		/// <param name="builder"></param>
		/// <returns>
		/// Returns the instance of the builder.
		/// </returns>
		public WebhookServiceBuilder<TSubscription> UseNotifier<TNotifier>()
			where TNotifier : class, IWebhookNotifier {
			Services.UseService<IWebhookNotifier, TNotifier>();
			return this;
		}

		/// <summary>
		/// Uses the default webhook notifier (and replaces an existing one).
		/// </summary>
		/// <param name="builder"></param>
		/// <returns></returns>
		public WebhookServiceBuilder<TSubscription> UseNotifier()
			=> UseNotifier<WebhookNotifier>();

		public WebhookServiceBuilder<TSubscription> AddDataFactory<TFactory>(ServiceLifetime lifetime = ServiceLifetime.Scoped)
			where TFactory : class, IWebhookDataFactory {
			Services.Add(new ServiceDescriptor(typeof(IWebhookDataFactory), typeof(TFactory), lifetime));
			Services.Add(new ServiceDescriptor(typeof(TFactory), typeof(TFactory), lifetime));
			return this;
		}

		public WebhookServiceBuilder<TSubscription> AddFilterEvaluator<TEvaluator>(ServiceLifetime lifetime = ServiceLifetime.Transient)
			where TEvaluator : class, IWebhookFilterEvaluator {
			Services.Add(new ServiceDescriptor(typeof(IWebhookFilterEvaluator), typeof(TEvaluator), lifetime));
			Services.Add(new ServiceDescriptor(typeof(TEvaluator), typeof(TEvaluator), lifetime));

			return this;
		}

		public WebhookServiceBuilder<TSubscription> UseSubscriptionResolver<TResolver>()
			where TResolver : class, IWebhookSubscriptionResolver {
			Services.UseService<IWebhookSubscriptionResolver, TResolver>();
			return this;
		}

		public WebhookServiceBuilder<TSubscription> UseSender<TSender>()
			where TSender : class, IWebhookSender {
			Services.UseService<IWebhookSender, TSender>();
			return this;
		}

		public WebhookServiceBuilder<TSubscription> UseDefaultSender()
			=> UseSender<WebhookSender>();

		public WebhookServiceBuilder<TSubscription> AddSerializer<TSerializer>(ServiceLifetime lifetime = ServiceLifetime.Singleton)
			where TSerializer : class, IWebhookSerializer {
			
			Services.Add(new ServiceDescriptor(typeof(IWebhookSerializer), typeof(TSerializer), lifetime));
			Services.Add(new ServiceDescriptor(typeof(TSerializer), typeof(TSerializer), lifetime));

			return this;
		}

		public WebhookServiceBuilder<TSubscription> AddSerializer<TSerializer>(TSerializer serializer)
			where TSerializer : class, IWebhookSerializer {
			Services.Add(new ServiceDescriptor(typeof(IWebhookSerializer), serializer));
			Services.Add(new ServiceDescriptor(typeof(TSerializer), serializer));

			return this;
		}


		public WebhookServiceBuilder<TSubscription> UseDefaultSubscriptionResolver()
			=> UseSubscriptionResolver<DefaultWebhookSubscriptionResolver<TSubscription>>();

		public WebhookServiceBuilder<TSubscription> UseSubscriptionManager<TManager>()
			where TManager : class, IWebhookSubscriptionManager<TSubscription> {
			Services.TryAddScoped<IWebhookSubscriptionResolver, DefaultWebhookSubscriptionResolver<TSubscription>>();

			Services.UseService<IWebhookSubscriptionManager<TSubscription>, TManager>();

			return this;
		}

		public WebhookServiceBuilder<TSubscription> UseSubscriptionManager()
			=> UseSubscriptionManager<WebhookSubscriptionManager<TSubscription>>();

	}
}
