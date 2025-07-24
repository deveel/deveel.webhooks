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

namespace Deveel.Webhooks {
	/// <summary>
	/// A builder that is used to configure the webhook sender.
	/// </summary>
	/// <typeparam name="TWebhook">
	/// The type of the webhook that is sent by the sender.
	/// </typeparam>
    public class WebhookSenderBuilder<TWebhook> where TWebhook : class {
		/// <summary>
		/// Creates a new instance of the builder.
		/// </summary>
		/// <param name="services">
		/// The collection of services that is used to register the webhook sender.
		/// </param>
		/// <exception cref="ArgumentNullException">
		/// Thrown when the <paramref name="services"/> argument is <c>null</c>.
		/// </exception>
		/// <remarks>
		/// This constructor registers the default services that are required to
		/// obtain a default webhook sender.
		/// </remarks>
		public WebhookSenderBuilder(IServiceCollection services) {
			Services = services ?? throw new ArgumentNullException(nameof(services));

			RegisterDefaultServices();
		}

		/// <summary>
		/// Gets the collection of services that is used to register the webhook sender.
		/// </summary>
        public IServiceCollection Services { get; }

        private void RegisterDefaultServices() {
			Services.TryAddScoped<IWebhookSender<TWebhook>, WebhookSender<TWebhook>>();
			Services.TryAddScoped<WebhookSender<TWebhook>>();

			Services.AddScoped<IWebhookDestinationVerifier<TWebhook>, WebhookDestinationVerifier<TWebhook>>();

			Services.AddHttpClient();
		}

		/// <summary>
		/// Registers the given sender service.
		/// </summary>
		/// <typeparam name="TSender">
		/// The type of the sender service that is registered.
		/// </typeparam>
		/// <param name="lifetime">
		/// The lifetime of the service that is registered (default to <see cref="ServiceLifetime.Scoped"/>)
		/// </param>
		/// <returns></returns>
		public WebhookSenderBuilder<TWebhook> UseSender<TSender>(ServiceLifetime lifetime = ServiceLifetime.Scoped)
			where TSender : class, IWebhookSender<TWebhook> {

			Services.RemoveAll<IWebhookSender<TWebhook>>();

			Services.Add(new ServiceDescriptor(typeof(IWebhookSender<TWebhook>), typeof(TSender), lifetime));
			Services.Add(new ServiceDescriptor(typeof(TSender), typeof(TSender), lifetime));

			return this;
		}

		/// <summary>
		/// Registers a service used to verify receivers of webhooks before
		/// attempting to deliver them.
		/// </summary>
		/// <returns>
		/// Returns the instance of the builder, to allow chaining
		/// </returns>
		public WebhookSenderBuilder<TWebhook> UseDestinationVerifier<TVerifier>()
			where TVerifier : class, IWebhookDestinationVerifier<TWebhook> {
			Services.RemoveAll<IWebhookDestinationVerifier<TWebhook>>();

			Services.AddScoped<IWebhookDestinationVerifier<TWebhook>, TVerifier>();
			Services.AddScoped<TVerifier>();

			return this;
		}
	}
}
