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

using System.Text;
using System.Text.Json;

using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace Deveel.Webhooks {
    /// <summary>
    /// An object that can be used to configure a receiver of webhooks
    /// </summary>
    /// <typeparam name="TWebhook">The type of webhooks to receive</typeparam>
    /// <remarks>
    /// When constructing the builder a set of default services are registered,
    /// such as the middleware for the receiver and the verifier, a default JSON
    /// parser and the default receiver service.
    /// </remarks>
    public sealed class WebhookReceiverBuilder<TWebhook> where TWebhook : class {
		/// <summary>
		/// Initializes a new instance of the <see cref="WebhookReceiverBuilder{TWebhook}"/> class
		/// </summary>
		/// <param name="services">
		/// The service collection to which the receiver is added
		/// </param>
		/// <exception cref="ArgumentException">
		/// Thrown if the type <typeparamref name="TWebhook"/> is not a non-abstract class
		/// </exception>
		/// <exception cref="ArgumentNullException">
		/// Thrown if the <paramref name="services"/> argument is <c>null</c>
		/// </exception>
		public WebhookReceiverBuilder(IServiceCollection services) {
			if (!typeof(TWebhook).IsClass || typeof(TWebhook).IsAbstract)
				throw new ArgumentException("The webhook type must be a non-abstract class");

			Services = services ?? throw new ArgumentNullException(nameof(services));

			Services.TryAddSingleton(this);

			RegisterReceiverMiddleware();
			RegisterDefaultReceiver();

			UseJsonParser();
		}

		/// <summary>
		/// Constructs a new instance of the <see cref="WebhookReceiverBuilder{TWebhook}"/> class
		/// instantiating a new service collection
		/// </summary>
		public WebhookReceiverBuilder()
			: this(new ServiceCollection()) {
        }

		/// <summary>
		/// Gets the service collection to which the receiver is added
		/// </summary>
		public IServiceCollection Services { get; }

		private void RegisterReceiverMiddleware() {
			Services.TryAddScoped<WebhookReceiverMiddleware<TWebhook>>();
		}

		private void RegisterVerifierMiddleware() {
			Services.TryAddScoped<WebhookRequestVerfierMiddleware<TWebhook>>();
		}

		private void RegisterDefaultReceiver() {
			Services.TryAddScoped<IWebhookReceiver<TWebhook>, WebhookReceiver<TWebhook>>();
			Services.TryAddScoped<WebhookReceiver<TWebhook>>();
		}

		/// <summary>
		/// Registers an implementation of the <see cref="IWebhookReceiver{TWebhook}"/>
		/// that is used to receive the webhooks
		/// </summary>
		/// <typeparam name="TReceiver">
		/// The type of the receiver to use for the webhooks of type <typeparamref name="TWebhook"/>
		/// </typeparam>
		/// <returns>
		/// Returns the current builder instance with the receiver registered
		/// </returns>
		public WebhookReceiverBuilder<TWebhook> UseReceiver<TReceiver>()
			where TReceiver : class, IWebhookReceiver<TWebhook> {

			Services.AddScoped<IWebhookReceiver<TWebhook>, TReceiver>();

			if (!typeof(TReceiver).IsAbstract)
				Services.AddScoped(typeof(TReceiver), typeof(TReceiver));

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
		public WebhookReceiverBuilder<TWebhook> UseVerifier<TVerifier>()
			where TVerifier : class, IWebhookRequestVerifier<TWebhook> {
			RegisterVerifierMiddleware();

			Services.AddScoped<IWebhookRequestVerifier<TWebhook>, TVerifier>();

			if (!typeof(TVerifier).IsAbstract)
				Services.AddScoped(typeof(TVerifier), typeof(TVerifier));

			return this;
		}

		/// <summary>
		/// Registers the default implementation of the <see cref="IWebhookRequestVerifier{TWebhook}"/>
		/// </summary>
		/// <param name="configure">
		/// A delegate that can be used to configure the options for the verifier
		/// </param>
		/// <returns>
		/// Returns the current builder instance with the verifier registered
		/// </returns>
		/// <seealso cref="WebhookRequestVerifier{TWebhook}"/>
		public WebhookReceiverBuilder<TWebhook> UseVerifier(Action<WebhookVerificationOptions> configure) {
			UseVerifier<WebhookRequestVerifier<TWebhook>>();

			Services.AddOptions<WebhookVerificationOptions>(typeof(TWebhook).Name)
				.Configure(configure);

			return this;
		}

		/// <summary>
		/// Registers the default implementation of the <see cref="IWebhookRequestVerifier{TWebhook}"/>
		/// </summary>
		/// <param name="sectionPath">
		/// The path to the section in the configuration that contains the options
		/// </param>
		/// <returns>
		/// Return the current builder instance with the verifier registered
		/// </returns>
		public WebhookReceiverBuilder<TWebhook> UserVerifier(string sectionPath) {
			UseVerifier<WebhookRequestVerifier<TWebhook>>();

			Services.AddOptions<WebhookVerificationOptions>(typeof(TWebhook).Name)
				.BindConfiguration(sectionPath);

			return this;
		}

		/// <summary>
		/// Registers an handler for the webhooks of type <typeparamref name="TWebhook"/>
		/// that were received.
		/// </summary>
		/// <typeparam name="THandler">
		/// The type of the handler to use for the webhooks of type <typeparamref name="TWebhook"/>
		/// </typeparam>
		/// <returns>
		/// Returns the current builder instance with the handler registered
		/// </returns>
		public WebhookReceiverBuilder<TWebhook> AddHandler<THandler>() 
			where THandler : class, IWebhookHandler<TWebhook> {

			Services.AddScoped<IWebhookHandler<TWebhook>, THandler>();

			if (typeof(THandler).IsClass && !typeof(THandler).IsAbstract)
				Services.AddScoped(typeof(THandler), typeof(THandler));

			return this;
		}

		/// <summary>
		/// Configures the receiver with the options from the given section path
		/// within the configuration of the application
		/// </summary>
		/// <param name="sectionPath">
		/// The path to the section within the configuration of the application
		/// where the options are defined
		/// </param>
		/// <returns>
		/// Returns the current builder instance with the options configured
		/// </returns>
		public WebhookReceiverBuilder<TWebhook> Configure(string sectionPath) {
			// TODO: Validate the configured options
			Services.AddOptions<WebhookReceiverOptions>(typeof(TWebhook).Name)
				.BindConfiguration(sectionPath);

			return this;
		}

		/// <summary>
		/// Configures the receiver with the given options
		/// </summary>
		/// <param name="configure">
		/// A delegate that is used to configure the options of the receiver
		/// </param>
		/// <returns>
		/// Returns the current builder instance with the options configured
		/// </returns>
		public WebhookReceiverBuilder<TWebhook> Configure(Action<WebhookReceiverOptions> configure) {
			// TODO: Validate the configured options
			Services.AddOptions<WebhookReceiverOptions>(typeof(TWebhook).Name)
				.Configure(configure);

			return this;
		}

		/// <summary>
		/// Registers a parser that is used to parse the JSON body of webhooks received
		/// </summary>
		/// <typeparam name="TParser">
		/// The type of the parser to use for the webhooks of type <typeparamref name="TWebhook"/>
		/// </typeparam>
		/// <param name="lifetime">
		/// A value that specifies the lifetime of the parser service (defaults to <see cref="ServiceLifetime.Singleton"/>)
		/// </param>
		/// <returns>
		/// Returns the current builder instance with the parser registered
		/// </returns>
		public WebhookReceiverBuilder<TWebhook> UseJsonParser<TParser>(ServiceLifetime lifetime = ServiceLifetime.Singleton)
			where TParser : class, IWebhookJsonParser<TWebhook> {

			Services.Add(new ServiceDescriptor(typeof(IWebhookJsonParser<TWebhook>), typeof(TParser), lifetime));

			if (typeof(TParser).IsClass && !typeof(TParser).IsAbstract)
				Services.Add(new ServiceDescriptor(typeof(TParser), typeof(TParser), lifetime));

			return this;
		}

		/// <summary>
		/// Registers a parser that is used to parse the JSON body of webhooks received
		/// </summary>
		/// <typeparam name="TParser">
		/// The type of the parser to use for the webhooks of type <typeparamref name="TWebhook"/>
		/// </typeparam>
		/// <param name="parser">
		/// An instance of the parser to use for the webhooks of type <typeparamref name="TWebhook"/>
		/// </param>
		/// <returns>
		/// Returns the current builder instance with the parser registered
		/// </returns>
		public WebhookReceiverBuilder<TWebhook> UseJsonParser<TParser>(TParser parser)
			where TParser : class, IWebhookJsonParser<TWebhook> {
			Services.AddSingleton(typeof(IWebhookJsonParser<TWebhook>), parser);
			Services.AddSingleton(parser);

			return this;
		}

		/// <summary>
		/// Registers a default parser that is used to parse the JSON body of webhooks received
		/// </summary>
		/// <param name="options">
		/// An optional set of options that are used to configure the JSON parser behavior
		/// </param>
		/// <returns>
		/// Returns the current builder instance with the parser registered
		/// </returns>
		public WebhookReceiverBuilder<TWebhook> UseJsonParser(JsonSerializerOptions? options = null) {
			Services.TryAddSingleton<IWebhookJsonParser<TWebhook>>(_ => new SystemTextWebhookJsonParser<TWebhook>(options));
			Services.TryAddSingleton(_ => new SystemTextWebhookJsonParser<TWebhook>(options));

			return this;
		}

		/// <summary>
		/// Registers a function as parser that is used to parse the JSON body of webhooks received
		/// </summary>
		/// <param name="parser">
		/// The function that is used to parse the JSON body of webhooks received
		/// </param>
		/// <returns>
		/// Returns the current builder instance with the parser registered
		/// </returns>
		/// <exception cref="ArgumentNullException">
		/// Thrown when the given <paramref name="parser"/> is <c>null</c>
		/// </exception>
		public WebhookReceiverBuilder<TWebhook> UseJsonParser(Func<Stream, CancellationToken, Task<TWebhook>> parser) {
            if (parser is null)
                throw new ArgumentNullException(nameof(parser));

            Services.TryAddSingleton<IWebhookJsonParser<TWebhook>>(_ => new DelegatedJsonParser(parser));

			return this;
		}

        /// <summary>
        /// Registers a function as parser that is used to parse the JSON body of webhooks received
        /// </summary>
        /// <param name="parser">
        /// The function that is used to parse the JSON body of webhooks received
        /// </param>
        /// <returns>
        /// Returns the current builder instance with the parser registered
        /// </returns>
        /// <exception cref="ArgumentNullException">
        /// Thrown when the given <paramref name="parser"/> is <c>null</c>
        /// </exception>
        public WebhookReceiverBuilder<TWebhook> UseJsonParser(Func<string, TWebhook> parser) {
            if (parser is null)
                throw new ArgumentNullException(nameof(parser));

            Services.TryAddSingleton<IWebhookJsonParser<TWebhook>>(_ => new DelegatedJsonParser(parser));

			return this;
		}

		/// <summary>
		/// Registers a service that is used to sign the payload of webhooks received
		/// </summary>
		/// <typeparam name="TSigner">
		/// The type of the signer to use for the webhooks of type <typeparamref name="TWebhook"/>
		/// </typeparam>
		/// <returns>
		/// Returns the current builder instance with the signer registered
		/// </returns>
		public WebhookReceiverBuilder<TWebhook> UseSigner<TSigner>() where TSigner : class, IWebhookSigner {			
			if (!typeof(IWebhookSigner<TWebhook>).IsAssignableFrom(typeof(TSigner))) {
				Services.AddSingleton<IWebhookSigner<TWebhook>>(provider => {
					var signer = provider.GetRequiredService<TSigner>();
					return new WebhookSignerWrapper(signer);
				});
			} else {
				Services.AddSingleton(provider => (IWebhookSigner<TWebhook>) provider.GetRequiredService<TSigner>());
			}

            Services.TryAddSingleton<TSigner>();
            Services.TryAddSingleton<IWebhookSignerProvider<TWebhook>, DefaultWebhookSignerProvider>();

			return this;
		}

		/// <summary>
		/// Registers the default implementation of <see cref="IWebhookSigner"/> that is used 
		/// to sign the payload of webhooks received with a SHA256 hash
		/// </summary>
		/// <returns>
		/// Returns the current builder instance with the signer registered
		/// </returns>
		public WebhookReceiverBuilder<TWebhook> UseSha256Signer()
			=> UseSigner<Sha256WebhookSigner>();

		/// <summary>
		/// Registers a service that is used to sign the payload of webhooks received
		/// </summary>
		/// <typeparam name="TSigner">
		/// The type of the signer to use for the webhooks of type <typeparamref name="TWebhook"/>
		/// </typeparam>
		/// <param name="signer">
		/// The instance of the signer to use for the webhooks of type <typeparamref name="TWebhook"/>
		/// </param>
		/// <returns>
		/// Returns the current builder instance with the signer registered
		/// </returns>
		/// <exception cref="ArgumentNullException">
		/// Thrown when the given <paramref name="signer"/> is <c>null</c>
		/// </exception>
		public WebhookReceiverBuilder<TWebhook> UseSigner<TSigner>(TSigner signer)
			where TSigner : class, IWebhookSigner {
            if (signer is null)
                throw new ArgumentNullException(nameof(signer));

            if (!typeof(IWebhookSigner<TWebhook>).IsAssignableFrom(typeof(TSigner))) {
				Services.AddSingleton<IWebhookSigner<TWebhook>>(_ => new WebhookSignerWrapper(signer));
			} else {
				Services.AddSingleton(provider => (IWebhookSigner<TWebhook>) provider.GetRequiredService<TSigner>());
			}

			Services.TryAddSingleton(signer);
            Services.TryAddSingleton<IWebhookSignerProvider<TWebhook>, DefaultWebhookSignerProvider>();

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

        #region WebhookSignatureProviderWrapper

        class WebhookSignerWrapper : IWebhookSigner<TWebhook> {
			private readonly IWebhookSigner signer;

			public WebhookSignerWrapper(IWebhookSigner signer) {
				this.signer = signer;
			}

			public string[] Algorithms => signer.Algorithms;

			public string SignWebhook(string webhookBody, string secret) => signer.SignWebhook(webhookBody, secret);
		}

		#endregion

		#region DelegatedJsonParser

		class DelegatedJsonParser : IWebhookJsonParser<TWebhook> {
			private readonly Func<Stream, CancellationToken, Task<TWebhook>>? streamParser;
			private readonly Func<string, TWebhook>? syncStringParser;

			public DelegatedJsonParser(Func<string, TWebhook> syncStringParser) {
				this.syncStringParser = syncStringParser;
			}

			public DelegatedJsonParser(Func<Stream, CancellationToken, Task<TWebhook>> parser) {
				this.streamParser = parser;
			}

			public async Task<TWebhook?> ParseWebhookAsync(Stream utf8Stream, CancellationToken cancellationToken = default) {
				if (streamParser != null) {
					return await streamParser(utf8Stream, cancellationToken);
				} else if (syncStringParser != null) {
					using var reader = new StreamReader(utf8Stream, Encoding.UTF8);
					var json = await reader.ReadToEndAsync();

					return syncStringParser(json);
				}

				return null;
			}
		}

		#endregion
	}
}
