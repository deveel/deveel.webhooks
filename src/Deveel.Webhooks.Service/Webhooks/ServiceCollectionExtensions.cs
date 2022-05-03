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

using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace Deveel.Webhooks {
	public static class ServiceCollectionExtensions {
		internal static IServiceCollection UseService<TService, TImplementation>(this IServiceCollection services, ServiceLifetime lifetime = ServiceLifetime.Scoped)
			where TService : class
			where TImplementation : class, TService {
			services.RemoveAll<TService>();
			services.Add(new ServiceDescriptor(typeof(TService), typeof(TImplementation), lifetime));
			services.Add(new ServiceDescriptor(typeof(TImplementation), typeof(TImplementation), lifetime));

			return services;
		}

		/// <summary>
		/// Adds the default services to support the webhook
		/// management provided by the framework.
		/// </summary>
		/// <param name="services">The collection of services</param>
		/// <param name="configure">A builder used to configure the service</param>
		/// <returns></returns>
		public static IServiceCollection AddWebhooks<TSubscription>(this IServiceCollection services, Action<WebhookServiceBuilder<TSubscription>> configure = null) 
			where TSubscription : class, IWebhookSubscription {

			var builder = services.AddWebhooks<TSubscription>();

			if (configure != null) {
				configure.Invoke(builder);
			}

			//} else {
			//	// add the default configurations ...
			//	services.AddOptions<WebhookDeliveryOptions>();
			//}

			return services;
		}

		public static WebhookServiceBuilder<TSubscription> AddWebhooks<TSubscription>(this IServiceCollection services) 
			where TSubscription : class, IWebhookSubscription {
			var builder = new WebhookServiceBuilder<TSubscription>(services);
			builder.AddDefaults();

			return builder;
		}

	}
}