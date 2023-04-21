﻿// Copyright 2022 Deveel
// 
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
// 
//     http://www.apache.org/licenses/LICENSE-2.0
// 
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.


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
			Services.TryAddScoped<IWebhookNotifier<Webhook>, WebhookNotifier<Webhook>>();
			Services.AddScoped<WebhookNotifier<Webhook>>();

			Services.TryAddScoped<IWebhookSender<Webhook>, WebhookSender<Webhook>>();
			Services.AddScoped<WebhookSender<Webhook>>();

			Services.AddSingleton<IWebhookFactory<Webhook>, DefaultWehbookFactory>();

			Services.AddSingleton<IWebhookSigner, Sha256WebhookSigner>();
			Services.AddSingleton<Sha256WebhookSigner>();

			Services.AddSingleton<IWebhookJsonSerializer<Webhook>, NewtonsoftWebhookJsonSerializer<Webhook>>();
			Services.AddSingleton<NewtonsoftWebhookJsonSerializer<Webhook>>();

			Services.AddScoped<IWebhookServiceConfiguration, WebhookServiceConfiguration>();

			Services.AddScoped<IWebhookSubscriptionManager<TSubscription>, WebhookSubscriptionManager<TSubscription>>();
			Services.AddScoped<IMultiTenantWebhookSubscriptionManager<TSubscription>, MultiTenantWebhookSubscriptionManager<TSubscription>>();
			Services.AddScoped<MultiTenantWebhookSubscriptionManager<TSubscription>>();

			// Services.AddOptions<WebhookDeliveryOptions>();

			Services.AddSingleton<IWebhookSubscriptionValidator<TSubscription>, WebhookSubscriptionValidator<TSubscription>>();
			Services.AddSingleton<IMultiTenantWebhookSubscriptionValidator<TSubscription>, MultiTenantWebhookSubscriptionValidator<TSubscription>>();
		}

		//public WebhookServiceBuilder<TSubscription> ConfigureDelivery(Action<WebhookDeliveryOptions> configure) {
		//	if (configure != null)
		//		Services.AddOptions<WebhookDeliveryOptions>().Configure(configure);

		//	return this;
		//}

		//public WebhookServiceBuilder<TSubscription> ConfigureDelivery(WebhookDeliveryOptions options) {
		//	Services.AddSingleton(options);
		//	return this;
		//}

		//public WebhookServiceBuilder<TSubscription>  ConfigureDelivery(string sectionName) {
		//	Services.AddOptions<WebhookDeliveryOptions>()
		//		.Configure<IConfiguration>((options, config) => {
		//			var section = config.GetSection(sectionName);
		//			if (section != null)
		//				section.Bind(options);
		//		});

		//	return this;
		//}

		//public WebhookServiceBuilder<TSubscription> ConfigureDelivery(Action<IWebhookDeliveryOptionsBuilder> options) {
		//	Services
		//		.AddOptions<WebhookDeliveryOptions>()
		//		.Configure(opts => {
		//			var optionsBuilder = new WebhookDeliveryOptionsBuilder(opts);
		//			options(optionsBuilder);
		//		});

		//	return this;
		//}

		//public WebhookServiceBuilder<TSubscription> ConfigureDefaultDelivery() {
		//	Services.AddOptions<WebhookDeliveryOptions>();
		//	return this;
		//}

		/// <summary>
		/// Replaces the notifier service with another one.
		/// </summary>
		/// <typeparam name="TNotifier">The type of the new <see cref="IWebhookNotifier"/> to be used</typeparam>
		/// <param name="builder"></param>
		/// <returns>
		/// Returns the instance of the builder.
		/// </returns>
		public WebhookServiceBuilder<TSubscription> UseNotifier<TWebhook, TNotifier>()
			where TWebhook : class, IWebhook
			where TNotifier : class, IWebhookNotifier<TWebhook> {
			Services.UseService<IWebhookNotifier<TWebhook>, TNotifier>();
			return this;
		}

		/// <summary>
		/// Uses the default webhook notifier (and replaces an existing one).
		/// </summary>
		/// <param name="builder"></param>
		/// <returns></returns>
		public WebhookServiceBuilder<TSubscription> UseDefaultNotifier<TWebhook>()
			where TWebhook : class, IWebhook
			=> UseNotifier<TWebhook, WebhookNotifier<TWebhook>>();

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

		public WebhookServiceBuilder<TSubscription> UseSender<TSender, TWebhook>()
			where TSender : class, IWebhookSender<TWebhook>
			where TWebhook : class {
			Services.UseService<IWebhookSender<TWebhook>, TSender>();
			return this;
		}

		public WebhookServiceBuilder<TSubscription> UseDefaultSender<TWebhook>()
			where TWebhook : class, IWebhook
			=> UseSender<WebhookSender<TWebhook>, TWebhook>();

		public WebhookServiceBuilder<TSubscription> AddSerializer<TWebhook, TSerializer>(ServiceLifetime lifetime = ServiceLifetime.Singleton)
			where TWebhook : class, IWebhook
			where TSerializer : class, IWebhookJsonSerializer<TWebhook> {
			
			Services.Add(new ServiceDescriptor(typeof(IWebhookJsonSerializer<TWebhook>), typeof(TSerializer), lifetime));
			Services.Add(new ServiceDescriptor(typeof(TSerializer), typeof(TSerializer), lifetime));

			return this;
		}

		public WebhookServiceBuilder<TSubscription> AddSerializer<TWebhook, TSerializer>(TSerializer serializer) 
			where TWebhook : class, IWebhook
			where TSerializer : class, IWebhookJsonSerializer<TWebhook> {
			Services.Add(new ServiceDescriptor(typeof(IWebhookJsonSerializer<TWebhook>), serializer));
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


		public WebhookServiceBuilder<TSubscription> AddSubscriptionValidator<TValidator>(ServiceLifetime lifetime = ServiceLifetime.Singleton)
			where TValidator : class, IWebhookSubscriptionValidator<TSubscription> {
			Services.Add(new ServiceDescriptor(typeof(IWebhookSubscriptionValidator<TSubscription>), typeof(TValidator), lifetime));
			Services.Add(new ServiceDescriptor(typeof(TValidator), typeof(TValidator), lifetime));

			return this;
		}
	}
}
