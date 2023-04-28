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
	/// A builder for configuring the services used to verify
	/// the webhook receiver application from the senders.
	/// </summary>
	/// <typeparam name="TWebhook"></typeparam>
	public sealed class WebhookVerifierBuilder<TWebhook> where TWebhook : class {
		/// <summary>
		/// Constructs the builder with the given services collection
		/// that will be used to register the services.
		/// </summary>
		/// <param name="services">
		/// The services collection to use to register the services
		/// for the verification of the receiver.
		/// </param>
		/// <exception cref="ArgumentNullException">
		/// Thrown when the given <paramref name="services"/> is <c>null</c>.
		/// </exception>
		public WebhookVerifierBuilder(IServiceCollection services) {
			Services = services ?? throw new ArgumentNullException(nameof(services));

			RegisterDefaultServices();
		}

		/// <summary>
		/// Gets the services collection used to register the services
		/// </summary>
		public IServiceCollection Services { get; }

		private void RegisterDefaultServices() {
			Services.TryAddScoped<IWebhookRequestVerifier<TWebhook>, WebhookRequestVerifier<TWebhook>>();
		}

		/// <summary>
		/// Configures the options for the verification of the webhooks receivers
		/// </summary>
		/// <param name="configure">
		/// A function that configures the options for the verification of the
		/// receivers of the webhooks.
		/// </param>
		/// <returns>
		/// Returns the current builder instance with the options configured.
		/// </returns>
		public WebhookVerifierBuilder<TWebhook> Configure(Action<WebhookVerificationOptions<TWebhook>> configure) {
			Services.AddOptions<WebhookVerificationOptions<TWebhook>>()
				.Configure(configure);

			return this;
		}

		/// <summary>
		/// Configures the options for the verification of the webhooks receivers
		/// using the given configuration section.
		/// </summary>
		/// <param name="sectionPath">
		/// The path to the configuration section that contains the options
		/// to be configured.
		/// </param>
		/// <returns>
		/// Returns the current builder instance with the options configured.
		/// </returns>
		public WebhookVerifierBuilder<TWebhook> Configure(string sectionPath) {
			Services.AddOptions<WebhookVerificationOptions<TWebhook>>()
				.BindConfiguration(sectionPath);

			return this;
		}

		/// <summary>
		/// Registers an implementation of the <see cref="IWebhookRequestVerifier{TWebhook}"/>
		/// that is used to verify the webhooks verification requests from
		/// senders
		/// </summary>
		/// <typeparam name="TVerifier">
		/// The type of the verifier to use for the webhooks of type <typeparamref name="TWebhook"/>
		/// </typeparam>
		/// <returns>
		/// Returns the current builder instance with the verifier registered
		/// </returns>
		public WebhookVerifierBuilder<TWebhook> UseVerifier<TVerifier>(ServiceLifetime lifetime = ServiceLifetime.Scoped)
			where TVerifier : class, IWebhookRequestVerifier<TWebhook> {

			Services.AddScoped<IWebhookRequestVerifier<TWebhook>, TVerifier>();
			Services.AddScoped(typeof(TVerifier), typeof(TVerifier));

			return this;
		}
	}
}
