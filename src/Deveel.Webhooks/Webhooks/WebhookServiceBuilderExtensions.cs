// Copyright 2022 Deveel
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

using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace Deveel.Webhooks {
	public static class WebhookServiceBuilderExtensions {
		public static IWebhookServiceBuilder Use<TService, TImplementation>(this IWebhookServiceBuilder builder, ServiceLifetime lifetime = ServiceLifetime.Scoped)
			where TService : class
			where TImplementation : class, TService {
			builder.ConfigureServices(services => services.UseService<TService, TImplementation>(lifetime));
			return builder;
		}

		public static IWebhookServiceBuilder ConfigureDelivery(this IWebhookServiceBuilder builder, Action<WebhookDeliveryOptions> configure) {
			if (configure != null)
				builder.ConfigureServices(services => services.AddOptions<WebhookDeliveryOptions>().Configure(configure));

			return builder;
		}

		public static IWebhookServiceBuilder ConfigureDelivery(this IWebhookServiceBuilder builder, WebhookDeliveryOptions options) {
			builder.ConfigureServices(services => services.AddSingleton(options));
			return builder;
		}

		public static IWebhookServiceBuilder ConfigureDelivery(this IWebhookServiceBuilder builder, string sectionName) {
			builder.ConfigureServices(services => services
				.AddOptions<WebhookDeliveryOptions>()
				.Configure<IConfiguration>((options, config) => {
					var section = config.GetSection(sectionName);
					if (section != null)
						section.Bind(options);
				}));

			return builder;
		}

		public static IWebhookServiceBuilder ConfigureDelivery(this IWebhookServiceBuilder builder, Action<IWebhookDeliveryOptionsBuilder> options) {
			builder.ConfigureServices(services => services
				.AddOptions<WebhookDeliveryOptions>()
				.Configure(opts => {
					var optionsBuilder = new WebhookDeliveryOptionsBuilder(opts);
					options(optionsBuilder);
				}));

			return builder;
		}

		/// <summary>
		/// Replaces the notifier service with another one.
		/// </summary>
		/// <typeparam name="TNotifier">The type of the new <see cref="IWebhookNotifier"/> to be used</typeparam>
		/// <param name="builder"></param>
		/// <returns>
		/// Returns the instance of the builder.
		/// </returns>
		public static IWebhookServiceBuilder UseNotifier<TNotifier>(this IWebhookServiceBuilder builder)
			where TNotifier : class, IWebhookNotifier
			=> builder.Use<IWebhookNotifier, TNotifier>();

		/// <summary>
		/// Uses the default webhook notifier (and replaces an existing one).
		/// </summary>
		/// <param name="builder"></param>
		/// <returns></returns>
		public static IWebhookServiceBuilder UseNotifier(this IWebhookServiceBuilder builder)
			=> builder.UseNotifier<WebhookNotifier>();

		public static IWebhookServiceBuilder AddDataFactory<TFactory>(this IWebhookServiceBuilder builder, ServiceLifetime lifetime = ServiceLifetime.Scoped)
			where TFactory : class, IWebhookDataFactory {
			builder.ConfigureServices(services => {
				services.Add(new ServiceDescriptor(typeof(IWebhookDataFactory), typeof(TFactory), lifetime));
				services.Add(new ServiceDescriptor(typeof(TFactory), typeof(TFactory), lifetime));
			});
			return builder;
		}

		public static IWebhookServiceBuilder AddFilterEvaluator<TEvaluator>(this IWebhookServiceBuilder builder, ServiceLifetime lifetime = ServiceLifetime.Transient)
			where TEvaluator : class, IWebhookFilterEvaluator {
			builder.ConfigureServices(services => {
				services.Add(new ServiceDescriptor(typeof(IWebhookFilterEvaluator), typeof(TEvaluator), lifetime));
				services.Add(new ServiceDescriptor(typeof(TEvaluator), typeof(TEvaluator), lifetime));
			});

			return builder;
		}

		public static IWebhookServiceBuilder UseSubscriptionResolver<TResolver>(this IWebhookServiceBuilder builder)
			where TResolver : class, IWebhookSubscriptionResolver
			=> builder.Use<IWebhookSubscriptionResolver, TResolver>();

		public static IWebhookServiceBuilder UseSender<TSender>(this IWebhookServiceBuilder builder)
			where TSender : class, IWebhookSender
			=> builder.Use<IWebhookSender, TSender>();

		public static IWebhookServiceBuilder UseDefaultSender(this IWebhookServiceBuilder builder)
			=> builder.UseSender<WebhookSender>();

		public static IWebhookServiceBuilder AddSerializer<TSerializer>(this IWebhookServiceBuilder builder, ServiceLifetime lifetime = ServiceLifetime.Singleton) 
			where TSerializer : class, IWebhookSerializer {
			builder.ConfigureServices(services => {
				services.Add(new ServiceDescriptor(typeof(IWebhookSerializer), typeof(TSerializer), lifetime));
				services.Add(new ServiceDescriptor(typeof(TSerializer), typeof(TSerializer), lifetime));
			});

			return builder;
		}

		public static IWebhookServiceBuilder AddSerializer<TSerializer>(this IWebhookServiceBuilder builder, TSerializer serializer)
			where TSerializer : class, IWebhookSerializer {
			builder.ConfigureServices(services => {
				services.Add(new ServiceDescriptor(typeof(IWebhookSerializer), serializer));
				services.Add(new ServiceDescriptor(typeof(TSerializer), serializer));
			});

			return builder;
		}
	}
}
