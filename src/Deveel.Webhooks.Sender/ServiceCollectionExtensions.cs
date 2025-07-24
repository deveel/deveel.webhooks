// Copyright 2022-2025 Antonello Provenzano
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

using Deveel.Webhooks;

using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Options;

namespace Deveel {
    /// <summary>
    /// Extends the <see cref="IServiceCollection"/> interface with methods
    /// to register the webhook sender services.
    /// </summary>
    public static class ServiceCollectionExtensions {
		/// <summary>
		/// Adds the webhook sender services to the given <paramref name="services"/>
		/// </summary>
		/// <typeparam name="TWebhook">
		/// The type of the webhook handled by the sender.
		/// </typeparam>
		/// <param name="services">
		/// The service collection to which the webhook sender services are added.
		/// </param>
		/// <returns>
		/// Returns a <see cref="WebhookSenderBuilder{TWebhook}"/> that can be used to
		/// further configure the webhook sender.
		/// </returns>
		public static WebhookSenderBuilder<TWebhook> AddWebhookSender<TWebhook>(this IServiceCollection services)
			where TWebhook : class {
			var builder = new WebhookSenderBuilder<TWebhook>(services);

			services.TryAddSingleton(builder);
			services.AddOptions<WebhookSenderOptions<TWebhook>>();

			return builder;
		}

		/// <summary>
		/// Adds the webhook sender services to the given <paramref name="services"/> with
		/// and initial configuration of the options.
		/// </summary>
		/// <typeparam name="TWebhook">
		/// The type of the webhook handled by the sender.
		/// </typeparam>
		/// <param name="services">
		/// The service collection to which the webhook sender services are added.
		/// </param>
		/// <param name="configure">
		/// A function that can be used to configure the webhook sender options.
		/// </param>
		/// <returns>
		/// Returns an instance of the <see cref="WebhookSenderBuilder{TWebhook}"/> that can be used
		/// to further configure the webhook sender.
		/// </returns>
		public static WebhookSenderBuilder<TWebhook> AddWebhookSender<TWebhook>(this IServiceCollection services, Action<WebhookSenderOptions<TWebhook>> configure)
			where TWebhook : class {
			services.AddOptions<WebhookSenderOptions<TWebhook>>()
								.Configure(configure);

			return services.AddWebhookSender<TWebhook>();
		}

		/// <summary>
		/// Adds the webhook sender services to the given <paramref name="services"/> with
		/// the options configured from the given <paramref name="sectionPath"/> within
		/// the underlying configuration of the application.
		/// </summary>
		/// <typeparam name="TWebhook">
		/// The type of the webhook handled by the sender.
		/// </typeparam>
		/// <param name="services">
		/// The service collection to which the webhook sender services are added.
		/// </param>
		/// <param name="sectionPath">
		/// A path to the section of the configuration that contains the options for the
		/// sender service.
		/// </param>
		/// <returns>
		/// Returns an instance of the <see cref="WebhookSenderBuilder{TWebhook}"/> that can be used
		/// to further configure the webhook sender.
		/// </returns>
		public static WebhookSenderBuilder<TWebhook> AddWebhookSender<TWebhook>(this IServiceCollection services, string sectionPath)
			where TWebhook : class {
			services.AddOptions<WebhookSenderOptions<TWebhook>>()
				.BindConfiguration(sectionPath);

			return services.AddWebhookSender<TWebhook>();
		}

		/// <summary>
		/// 
		/// </summary>
		/// <typeparam name="TWebhook"></typeparam>
		/// <param name="services"></param>
		/// <param name="options"></param>
		/// <returns></returns>
		public static WebhookSenderBuilder<TWebhook> AddWebhookSender<TWebhook>(this IServiceCollection services, WebhookSenderOptions<TWebhook> options)
			where TWebhook : class {

			services.AddSingleton(Options.Create(options));

			return services.AddWebhookSender<TWebhook>();
		}

		/// <summary>
		/// Adds the webhook sender services to the given <paramref name="services"/>
		/// </summary>
		/// <typeparam name="TWebhook">
		/// The type of the webhook handled by the sender.
		/// </typeparam>
		/// <param name="services">
		/// The service collection to which the webhook sender services are added.
		/// </param>
		/// <param name="configure">
		/// A function that can be used to further configure the webhook sender.
		/// </param>
		/// <returns>
		/// Returns the instance of the <paramref name="services"/> collection with
		/// the webhook sender services added.
		/// </returns>
		public static IServiceCollection AddWebhookSender<TWebhook>(this IServiceCollection services, Action<WebhookSenderBuilder<TWebhook>> configure)
			where TWebhook : class {
			var builder = services.AddWebhookSender<TWebhook>();
			configure?.Invoke(builder);

			return services;
		}
	}
}
