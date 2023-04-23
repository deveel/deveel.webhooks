// Copyright 2022-2023 Deveel
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
	/// <summary>
	/// A set of extension methods to add the webhook subscription
	/// management to the service collection.
	/// </summary>
	public static class ServiceCollectionExtensions {
		/// <summary>
		/// Adds the default services to support the webhook
		/// management provided by the framework.
		/// </summary>
		/// <typeparam name="TSubscription">
		/// The type of the subscription that is managed by the service.
		/// </typeparam>
		/// <param name="services">The collection of services</param>
		/// <param name="configure">A builder used to configure the service</param>
		/// <returns>
		/// Returns the collection where the webhook management service is registered.
		/// </returns>
		public static IServiceCollection AddWebhookSubscriptions<TSubscription>(this IServiceCollection services, Action<WebhookSubscriptionBuilder<TSubscription>>? configure = null) 
			where TSubscription : class, IWebhookSubscription {

			var builder = services.AddWebhooksSubscriptions<TSubscription>();
			configure?.Invoke(builder);

			return services;
		}

		/// <summary>
		/// Adds the default services to support the webhook
		/// management provided by the framework.
		/// </summary>
		/// <typeparam name="TSubscription">
		/// The type of the subscription that is managed by the service.
		/// </typeparam>
		/// <param name="services">
		/// The collection where the service is registered.
		/// </param>
		/// <returns>
		/// Returns the builder used to configure the service.
		/// </returns>
		public static WebhookSubscriptionBuilder<TSubscription> AddWebhooksSubscriptions<TSubscription>(this IServiceCollection services) 
			where TSubscription : class, IWebhookSubscription {
			var builder = new WebhookSubscriptionBuilder<TSubscription>(services);

			services.TryAddSingleton(builder);

			return builder;
		}

	}
}