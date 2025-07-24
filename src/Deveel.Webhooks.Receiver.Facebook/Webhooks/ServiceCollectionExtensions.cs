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

using Deveel.Webhooks.Facebook;

using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

namespace Deveel.Webhooks {
	/// <summary>
	/// Extends the <see cref="IServiceCollection"/> to add the Facebook Webhook receiver
	/// </summary>
    public static class ServiceCollectionExtensions {
		/// <summary>
		/// Adds the default configuration for the Facebook Webhook receiver
		/// </summary>
		/// <param name="services">
		/// The collection of services to which the receiver is added
		/// </param>
		/// <remarks>
		/// This meth adds the default configuration for the Facebook Webhook receiver
		/// and registers a default instance of the end-point verifier for the Facebook
		/// webhooks.
		/// </remarks>
		/// <returns>
		/// Returns an instance of <see cref="WebhookReceiverBuilder{FacebookWebhook}"/> that can
		/// be used to further configure the receiver.
		/// </returns>
		public static WebhookReceiverBuilder<FacebookWebhook> AddFacebookReceiver(this IServiceCollection services) {
			var builder = services.AddWebhookReceiver<FacebookWebhook>(_ => { });

			services.AddOptions<FacebookReceiverOptions>();

			services.AddWebhookVerifier<FacebookWebhook>()
				.UseVerifier<FacebookRequestVerifier>();

			services.AddTransient<IPostConfigureOptions<WebhookReceiverOptions<FacebookWebhook>>, ConfigureWebhookReceiverOptions>();
			services.AddTransient<IPostConfigureOptions<WebhookVerificationOptions<FacebookWebhook>>, ConfigureWebhookVerificationOptions>();

			return builder;
		}

		/// <summary>
		/// Adds a Facebook Webhook receiver to the service collection that is
		/// configured using the specified options.
		/// </summary>
		/// <param name="services">
		/// The collection of services to which the receiver is added
		/// </param>
		/// <param name="options">
		/// The options that are used to configure the receiver
		/// </param>
		/// <returns>
		/// Returns an instance of <see cref="WebhookReceiverBuilder{FacebookWebhook}"/> that can
		/// be used to further configure the receiver.
		/// </returns>
		public static WebhookReceiverBuilder<FacebookWebhook> AddFacebookReceiver(this IServiceCollection services, FacebookReceiverOptions options) {
			services.AddSingleton(Options.Create(options));

			return services.AddFacebookReceiver();
		}

		/// <summary>
		/// Adds a Facebook Webhook receiver to the service collection that is
		/// configured using the specified configuration section.
		/// </summary>
		/// <param name="services">
		/// The collection of services to which the receiver is added
		/// </param>
		/// <param name="sectionPath">
		/// The path to the configuration section that contains the Facebook-specific options 
		/// for the webhook receiver
		/// </param>
		/// <returns>
		/// Returns an instance of <see cref="WebhookReceiverBuilder{FacebookWebhook}"/> that can
		/// be used to further configure the receiver.
		/// </returns>
		/// <seealso cref="AddFacebookReceiver(IServiceCollection)"/>
		public static WebhookReceiverBuilder<FacebookWebhook> AddFacebookReceiver(this IServiceCollection services, string sectionPath) {
			services.AddOptions<FacebookReceiverOptions>()
				.BindConfiguration(sectionPath);

			return services.AddFacebookReceiver();
		}

        /// <summary>
        /// Adds a Facebook Webhook receiver to the service collection that is
        /// configured manually.
        /// </summary>
        /// <param name="services">
        /// The collection of services to which the receiver is added
        /// </param>
		/// <param name="configure">
		/// The delegate that is used to configure the Facebook-specific options
		/// </param>
        /// <returns>
        /// Returns an instance of <see cref="WebhookReceiverBuilder{FacebookWebhook}"/> that can
        /// be used to further configure the receiver.
        /// </returns>
		/// <seealso cref="AddFacebookReceiver(IServiceCollection)"/>
        public static WebhookReceiverBuilder<FacebookWebhook> AddFacebookReceiver(this IServiceCollection services, Action<FacebookReceiverOptions> configure) {
			services.Configure(configure);

			return services.AddFacebookReceiver();
		}
	}
}