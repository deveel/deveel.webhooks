using System;

using Microsoft.Extensions.DependencyInjection;

namespace Deveel.Webhooks {
    /// <summary>
    /// An object that is used to configure the webhooks service,
    /// providing a fluent API to configure the sub-services.
    /// </summary>
    /// <typeparam name="TWebhook">
    /// The type of the webhook that is handled by the service.
    /// </typeparam>
    /// <remarks>
    /// <para>
    /// The main goal of this class is to provide a fluent API to configure
    /// other services, such as the <see cref="IWebhookNotifier{TWebhook}"/>,
    /// the <see cref="IWebhookSender{TWebhook}"/>, aligning them in the
    /// bounded context of the given <typeparamref name="TWebhook"/>.
    /// </para>
    /// <para>
    /// This class is aiming to provide consistency in the configuration
    /// process of the webhooks service, by providing a single entry point
    /// for the configuration of the sub-services.
    /// </para>
    /// </remarks>
    public sealed class WebhooksBuilder<TWebhook> where TWebhook : class {
        /// <summary>
        /// Constructs the <see cref="WebhooksBuilder{TWebhook}"/> instance
        /// </summary>
        /// <param name="services">
        /// The service collection that is used to register the services
        /// </param>
        /// <exception cref="ArgumentNullException">
        /// Thrown when the given <paramref name="services"/> is <c>null</c>
        /// </exception>
        public WebhooksBuilder(IServiceCollection services) {
            Services = services ?? throw new ArgumentNullException(nameof(services));
        }

        /// <summary>
        /// Gets the service collection that is used to register the services
        /// </summary>
        public IServiceCollection Services { get; }

		/// <summary>
		/// Adds the notifier service to the webhooks service.
		/// </summary>
		/// <returns>
		/// Returns a <see cref="WebhookNotifierBuilder{TWebhook}"/> instance that
		/// can be used to configure the notifier service.
		/// </returns>
		public WebhooksBuilder<TWebhook> AddNotifier() { 
			Services.AddWebhookNotifier<TWebhook>();
			return this;
		}

        /// <summary>
        /// Adds the notifier service to the webhooks service.
        /// </summary>
        /// <param name="configure">
        /// A function that is used to configure the notifier service.
        /// </param>
        /// <returns>
        /// Returns the current <see cref="WebhooksBuilder{TWebhook}"/> instance
        /// for chaining.
        /// </returns>
        public WebhooksBuilder<TWebhook> AddNotifier(Action<WebhookNotifierBuilder<TWebhook>> configure) { 
            Services.AddWebhookNotifier(configure);
            return this;
        }

		/// <summary>
		/// Adds the sender service to the webhooks service.
		/// </summary>
		/// <returns>
		/// Returns a <see cref="WebhookSenderBuilder{TWebhook}"/> instance that
		/// can be used to configure the sender service.
		/// </returns>
		public WebhooksBuilder<TWebhook> AddSender() { 
			Services.AddWebhookSender<TWebhook>();
			return this;
		}

        /// <summary>
        /// Adds the sender service to the webhooks service.
        /// </summary>
        /// <param name="configure">
        /// A function that is used to configure the sender service.
        /// </param>
        /// <returns>
        /// Returns the current <see cref="WebhooksBuilder{TWebhook}"/> instance
        /// for chaining.
        /// </returns>
        public WebhooksBuilder<TWebhook> AddSender(Action<WebhookSenderBuilder<TWebhook>> configure) {
            Services.AddWebhookSender(configure);
            return this;
        }
    }
}
