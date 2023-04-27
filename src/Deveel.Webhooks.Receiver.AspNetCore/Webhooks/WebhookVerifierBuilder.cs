using System;

using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace Deveel.Webhooks {
	public sealed class WebhookVerifierBuilder<TWebhook> where TWebhook : class {
		public WebhookVerifierBuilder(IServiceCollection services) {
			Services = services ?? throw new ArgumentNullException(nameof(services));

			RegisterDefaultServices();
		}

		public IServiceCollection Services { get; }

		private void RegisterDefaultServices() {
			Services.TryAddScoped<IWebhookRequestVerifier<TWebhook>, WebhookRequestVerifier<TWebhook>>();
		}

		public WebhookVerifierBuilder<TWebhook> Configure(Action<WebhookVerificationOptions<TWebhook>> configure) {
			Services.AddOptions<WebhookVerificationOptions<TWebhook>>()
				.Configure(configure);

			return this;
		}

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
