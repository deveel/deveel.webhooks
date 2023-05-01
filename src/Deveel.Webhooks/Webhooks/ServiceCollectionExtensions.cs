﻿// Copyright 2022-2023 Deveel
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

using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Options;

namespace Deveel.Webhooks {
	/// <summary>
	/// Extends the <see cref="IServiceCollection"/> to add the features to notify webhooks.
	/// </summary>
	public static class ServiceCollectionExtensions {
		/// <summary>
		/// Adds a <see cref="WebhookNotifierBuilder{TWebhook}"/> to the service collection.
		/// </summary>
		/// <typeparam name="TWebhook">
		/// The type of the webhook to notify.
		/// </typeparam>
		/// <param name="services">
		/// The service collection to add the builder to.
		/// </param>
		/// <returns>
		/// Returns an instance of <see cref="WebhookNotifierBuilder{TWebhook}"/> that can be used
		/// to further configure the notifier.
		/// </returns>
		public static WebhookNotifierBuilder<TWebhook> AddWebhookNotifier<TWebhook>(this IServiceCollection services)
			where TWebhook : class {
			var builder = new WebhookNotifierBuilder<TWebhook>(services);

			services.TryAddSingleton(builder);

			return builder;
		}

		public static WebhookNotifierBuilder<TWebhook> AddWebhookNotifier<TWebhook>(this IServiceCollection services, WebhookNotificationOptions<TWebhook> options)
			where TWebhook : class {

			services.AddSingleton(Options.Create(options));

			return services.AddWebhookNotifier<TWebhook>();
		}

		public static WebhookNotifierBuilder<TWebhook> AddWebhookNotifier<TWebhook>(this IServiceCollection services, Action<WebhookNotificationOptions<TWebhook>> configure)
			where TWebhook : class {

			services.AddOptions<WebhookNotificationOptions<TWebhook>>()
				.Configure(configure);

			return services.AddWebhookNotifier<TWebhook>();
		}

		public static WebhookNotifierBuilder<TWebhook> AddWebhookNotifier<TWebhook>(this IServiceCollection services, string sectionPath)
			where TWebhook : class {

			services.AddOptions<WebhookNotificationOptions<TWebhook>>()
				.BindConfiguration(sectionPath);

			return services.AddWebhookNotifier<TWebhook>();
		}

		/// <summary>
		/// Adds a <see cref="WebhookNotifierBuilder{TWebhook}"/> to the service collection
		/// </summary>
		/// <typeparam name="TWebhook">
		/// The type of the webhook to notify.
		/// </typeparam>
		/// <param name="services">
		/// The service collection to add the builder to.
		/// </param>
		/// <param name="configure">
		/// A function used to configure the notification features.
		/// </param>
		/// <returns>
		/// Returns the instance of <see cref="IServiceCollection"/> to allow chaining of calls.
		/// </returns>
		public static IServiceCollection AddWebhookNotifier<TWebhook>(this IServiceCollection services, Action<WebhookNotifierBuilder<TWebhook>> configure)
			where TWebhook: class {
			var builder = services.AddWebhookNotifier<TWebhook>();
			configure?.Invoke(builder);

			return services;
		}
	}
}
