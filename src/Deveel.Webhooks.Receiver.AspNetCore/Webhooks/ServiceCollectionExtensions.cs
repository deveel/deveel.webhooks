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

using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Options;

namespace Deveel.Webhooks {
    /// <summary>
    /// Extends a <see cref="IServiceCollection"/> object to register
    /// a receiver of a specific type of webhooks.
    /// </summary>
    public static class ServiceCollectionExtensions {
		private static WebhookReceiverBuilder<TWebhook> AddWebhookReceiverCore<TWebhook>(this IServiceCollection services)
			where TWebhook : class {
			var builder = new WebhookReceiverBuilder<TWebhook>(services);
			services.TryAddSingleton(builder);
			return builder;
		}

		/// <summary>
		/// Configures a receiver without adding it to the service collection
		/// or registering any default services.
		/// </summary>
		/// <typeparam name="TWebhook">
		/// The type of webhooks to receive
		/// </typeparam>
		/// <param name="services">
		/// The service collection of which the receiver is part
		/// </param>
		/// <param name="configure">
		/// The delegate used to configure the receiver
		/// </param>
		/// <returns>
		/// Returns the collection of services.
		/// </returns>
		public static IServiceCollection ConfigureWebhookReceiver<TWebhook>(this IServiceCollection services, Action<WebhookReceiverBuilder<TWebhook>> configure)
			where TWebhook : class {
			var builder = new WebhookReceiverBuilder<TWebhook>(services);
			configure?.Invoke(builder);

			return services;
		}

		/// <summary>
		/// Adds a receiver of webhooks of a specific type to the service collection.
		/// </summary>
		/// <typeparam name="TWebhook">
		/// The type of webhooks to receive
		/// </typeparam>
		/// <param name="services">
		/// The service collection to which the receiver is added
		/// </param>
		/// <param name="options">
		/// The instance of <see cref="WebhookReceiverOptions{TWebhook}"/> that
		/// is used to configure the receiver.
		/// </param>
		/// <returns>
		/// Returns an instance of <see cref="WebhookReceiverBuilder{TWebhook}"/> that can
		/// be used to further configure the receiver.
		/// </returns>
		public static WebhookReceiverBuilder<TWebhook> AddWebhookReceiver<TWebhook>(this IServiceCollection services, WebhookReceiverOptions<TWebhook> options)
			where TWebhook : class {

			services.AddSingleton(Options.Create(options));

			return services.AddWebhookReceiverCore<TWebhook>();
		}

		/// <summary>
		/// Adds a receiver of webhooks of a specific type to the service collection.
		/// </summary>
		/// <typeparam name="TWebhook">
		/// The type of webhooks to receive
		/// </typeparam>
		/// <param name="services">
		/// The service collection to which the receiver is added
		/// </param>
		/// <param name="sectionPath">
		/// The path to the configuration section that contains the options for the webhook receiver
		/// </param>
		/// <returns>
		/// Returns an instance of <see cref="WebhookReceiverBuilder{TWebhook}"/> that can
		/// be used to further configure the receiver.
		/// </returns>
		public static WebhookReceiverBuilder<TWebhook> AddWebhookReceiver<TWebhook>(this IServiceCollection services, string sectionPath)
			where TWebhook : class {
			services.AddOptions<WebhookReceiverOptions<TWebhook>>()
				.BindConfiguration(sectionPath);

			return services.AddWebhookReceiverCore<TWebhook>();
		}

		/// <summary>
		/// Adds a receiver of webhooks of a specific type to the service collection.
		/// </summary>
		/// <typeparam name="TWebhook">
		/// The type of webhooks to receive
		/// </typeparam>
		/// <param name="services">
		/// The service collection to which the receiver is added
		/// </param>
		/// <param name="configure">
		/// A configuraton action that can be used to further configure the receiver
		/// </param>
		/// <returns>
		/// Returns an instance of <see cref="WebhookReceiverBuilder{TWebhook}"/> that can
		/// be used to further configure the receiver.
		/// </returns>
		public static WebhookReceiverBuilder<TWebhook> AddWebhookReceiver<TWebhook>(this IServiceCollection services, Action<WebhookReceiverOptions<TWebhook>> configure)
			where TWebhook : class {
			var builder = services.AddWebhookReceiverCore<TWebhook>();

			builder.Services.AddOptions<WebhookReceiverOptions<TWebhook>>()
				.Configure(configure);

			return builder;
		}

		///// <summary>
		///// Adds a receiver of webhooks of a specific type to the service collection.
		///// </summary>
		///// <typeparam name="TWebhook">
		///// The type of webhooks to receive
		///// </typeparam>
		///// <param name="services">
		///// The service collection to which the receiver is added
		///// </param>
		///// <param name="configure">
		///// A configuraton action that can be used to further configure the receiver
		///// </param>
		///// <returns>
		///// Returns an instance of <see cref="IServiceCollection"/> that can be used to register
		///// other services and configurations.
		///// </returns>
		//public static IServiceCollection AddWebhookReceiver<TWebhook>(this IServiceCollection services, Action<WebhookReceiverBuilder<TWebhook>> configure) 
		//	where TWebhook : class {
		//	var builder = services.AddWebhookReceiver<TWebhook>();
		//	configure?.Invoke(builder);

		//	return services;
		//}

		/// <summary>
		/// Registers the service that is handling the receiver verification
		/// requests made by the webhook sender.
		/// </summary>
		/// <typeparam name="TWebhook">
		/// The type of webhooks to receive
		/// </typeparam>
		/// <param name="services">
		/// The collection of services to which the verification service is added
		/// </param>
		/// <returns>
		/// Returns an instance of <see cref="WebhookVerifierBuilder{TWebhook}"/> that can
		/// be used to further configure the verification service.
		/// </returns>
		public static WebhookVerifierBuilder<TWebhook> AddWebhookVerifier<TWebhook>(this IServiceCollection services) 
			where TWebhook : class {
			var builder = new WebhookVerifierBuilder<TWebhook>(services);

			services.TryAddSingleton(builder);

			return builder;
		}

        /// <summary>
        /// Registers the service that is handling the receiver verification
        /// requests made by the webhook sender.
        /// </summary>
        /// <typeparam name="TWebhook">
        /// The type of webhooks to receive
        /// </typeparam>
        /// <param name="services">
        /// The collection of services to which the verification service is added
        /// </param>
		/// <param name="configure">
		/// A configuration action that can be used to further configure the verification service
		/// </param>
		/// <returns>
		/// Returns the collection of services with the registered
		/// verification service.
		/// </returns>
        public static IServiceCollection AddWebhookVerifier<TWebhook>(this IServiceCollection services, Action<WebhookVerifierBuilder<TWebhook>> configure)
			where TWebhook : class {
			var builder = services.AddWebhookVerifier<TWebhook>();

			configure?.Invoke(builder);

			return services;
		}
	}
}
