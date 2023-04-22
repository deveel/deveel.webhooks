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
			UseSender<WebhookSender<TWebhook>>();
			UseJsonSerializer<SystemTextWebhookJsonSerializer<TWebhook>>();
			UseSigner<Sha256WebhookSigner>();

			UseDestinationVerifier(_ => { });
		}

		/// <summary>
		/// Configures the webhook sender options using the section at
		/// the given <paramref name="sectionPath"/> within the
		/// underling configuration of the application.
		/// </summary>
		/// <param name="sectionPath">
		/// The path of the section within the configuration that
		/// defines the options configurations.
		/// </param>
		/// <returns>
		/// Returns the instance of the builder, to allow chaining
		/// </returns>
		public WebhookSenderBuilder<TWebhook> Configure(string sectionPath) {
			Services.AddOptions<WebhookSenderOptions>(typeof(TWebhook).Name)
				.BindConfiguration(sectionPath);

			return this;
		}

		/// <summary>
		/// Configures the webhook sender options using the given function
		/// </summary>
		/// <param name="configure">
		/// A function that is used to configure the sender options.
		/// </param>
		/// <returns>
		/// Returns the instance of the builder, to allow chaining
		/// </returns>
		public WebhookSenderBuilder<TWebhook> Configure(Action<WebhookSenderOptions> configure) {
			Services.AddOptions<WebhookSenderOptions>(typeof(TWebhook).Name)
				.Configure(configure);

			return this;
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

			Services.TryAdd(new ServiceDescriptor(typeof(IWebhookSender<TWebhook>), typeof(TSender), lifetime));
			Services.TryAdd(new ServiceDescriptor(typeof(TSender), typeof(TSender), lifetime));

			return this;
		}

		/// <summary>
		/// Registers a JSON serializer service.
		/// </summary>
		/// <typeparam name="TSerializer">
		/// The type of the serializer service that is registered.
		/// </typeparam>
		/// <returns>
		/// Returns the instance of the builder, to allow chaining
		/// </returns>
		public WebhookSenderBuilder<TWebhook> UseJsonSerializer<TSerializer>()
			where TSerializer : class, IWebhookJsonSerializer<TWebhook> {
			
			Services.TryAddSingleton<IWebhookJsonSerializer<TWebhook>, TSerializer>();
			Services.TryAddSingleton<TSerializer>();

			return this;
		}

		/// <summary>
		/// Registers a service used to sign webhook payloads.
		/// </summary>
		/// <typeparam name="TSigner">
		/// The type of the signer service that is registered.
		/// </typeparam>
		/// <returns>
		/// Returns the instance of the builder, to allow chaining
		/// </returns>
		public WebhookSenderBuilder<TWebhook> UseSigner<TSigner>()
			where TSigner : class, IWebhookSigner {

			if (typeof(IWebhookSigner<TWebhook>).IsAssignableFrom(typeof(TSigner))) {
				Services.TryAddSingleton<IWebhookSigner<TWebhook>>(provider => 
					(IWebhookSigner<TWebhook>) provider.GetRequiredService<TSigner>());
			} else {
				Services.TryAddScoped<IWebhookSigner<TWebhook>>(provider => {
					var signer = provider.GetRequiredService<TSigner>();
					return new WebhookSignerAdapter(signer);
				});
			}

			Services.TryAddSingleton<TSigner>();
            Services.TryAddSingleton<IWebhookSignerProvider<TWebhook>, DefaultWebhookSignerProvider>();

            return this;
		}

		/// <summary>
		/// Registers a service used to verify receivers of webhooks before
		/// attempting to deliver them.
		/// </summary>
		/// <param name="sectionPath">
		/// The path of the section within the configuration that
		/// is used to configure the verifier.
		/// </param>
		/// <returns>
		/// Returns the instance of the builder, to allow chaining
		/// </returns>
		public WebhookSenderBuilder<TWebhook> UseDestinationVerifier(string sectionPath) {
			Services.AddOptions<WebhookDestinationVerifierOptions>(typeof(TWebhook).Name)
				.BindConfiguration(sectionPath);

			Services.TryAddScoped<IWebhookDestinationVerifier<TWebhook>, WebhookDestinationVerifier<TWebhook>>();
			Services.TryAddScoped<WebhookDestinationVerifier<TWebhook>>();

			return this;
		}

		/// <summary>
		/// Registers a service used to verify receivers of webhooks before
		/// attempting to deliver them.
		/// </summary>
		/// <param name="configure"></param>
		/// <returns></returns>
		public WebhookSenderBuilder<TWebhook> UseDestinationVerifier(Action<WebhookDestinationVerifierOptions> configure) {
			Services.AddOptions<WebhookDestinationVerifierOptions>(typeof(TWebhook).Name)
				.Configure(configure);

			Services.TryAddScoped<IWebhookDestinationVerifier<TWebhook>, WebhookDestinationVerifier<TWebhook>>();
			Services.TryAddScoped<WebhookDestinationVerifier<TWebhook>>();

			return this;
		}

        #region DefaultWebhookSignerProvider

        class DefaultWebhookSignerProvider : IWebhookSignerProvider<TWebhook> {
            private readonly IDictionary<string, IWebhookSigner> signers;

            public DefaultWebhookSignerProvider(IEnumerable<IWebhookSigner<TWebhook>> signers) {
                this.signers = new Dictionary<string, IWebhookSigner>(StringComparer.OrdinalIgnoreCase);

                if (signers != null) {
                    foreach (var signer in signers) {
                        foreach (var alg in signer.Algorithms) {
                            this.signers[alg] = signer;
                        }
                    }
                }
            }

            public IWebhookSigner? GetSigner(string algorithm) {
                if (!signers.TryGetValue(algorithm, out var signer))
                    return null;

                return signer;
            }
        }

        #endregion

        private class WebhookSignerAdapter : IWebhookSigner<TWebhook> {
			private IWebhookSigner signer;

			public WebhookSignerAdapter(IWebhookSigner signer) {
				this.signer = signer;
			}

			public string[] Algorithms => signer.Algorithms;

			public string SignWebhook(string webhookBody, string secret) => signer.SignWebhook(webhookBody, secret);
		}
	}
}
