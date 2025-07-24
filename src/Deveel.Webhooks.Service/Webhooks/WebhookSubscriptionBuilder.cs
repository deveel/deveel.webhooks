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

using Deveel.Data;

using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace Deveel.Webhooks {
	/// <summary>
	/// An object that is used to configure the webhook services.
	/// </summary>
	/// <typeparam name="TSubscription">
	/// The type of the subscription that is used to notify webhooks.
	/// </typeparam>
	/// <typeparam name="TKey">
	/// The type of the key used to identify the subscription.
	/// </typeparam>
	public sealed class WebhookSubscriptionBuilder<TSubscription, TKey> 
		where TSubscription : class, IWebhookSubscription 
		where TKey : notnull {
		/// <summary>
		/// Initializes a new instance of the <see cref="WebhookSubscriptionBuilder{TSubscription,TKey}"/> class.
		/// </summary>
		/// <param name="services">
		/// The collection of services that are used to configure the webhook services.
		/// </param>
		/// <remarks>
		/// This constructor registers a set of default services that are used to
		/// run a webhook service.
		/// </remarks>
		/// <exception cref="ArgumentNullException">
		/// Thrown when the <paramref name="services"/> argument is <c>null</c>.
		/// </exception>
		public WebhookSubscriptionBuilder(IServiceCollection services) {
			Services = services ?? throw new ArgumentNullException(nameof(services));

			RegisterDefaults();
		}

		/// <summary>
		/// Gets the collection of services that are used to configure the webhook subscription service.
		/// </summary>
		public IServiceCollection Services { get; }

		private void RegisterDefaults() {
			Services.TryAddScoped<WebhookSubscriptionManager<TSubscription, TKey>>();
			Services.TryAddSingleton<IWebhookSubscriptionValidator<TSubscription, TKey>, WebhookSubscriptionValidator<TSubscription, TKey>>();

			Services.TryAddScoped<IWebhookSubscriptionResolver, WebhookSubscriptionResolver<TSubscription, TKey>>();
			Services.TryAddScoped<WebhookSubscriptionResolver<TSubscription, TKey>>();
			// Services.TryAddScoped<ITenantWebhookSubscriptionResolver, TenantWebhookSubscriptionResolver<TSubscription>>();
		}

		/// <summary>
		/// Registers a custom <see cref="WebhookSubscriptionManager{TSubscription,TKey}"/>
		/// that overrides the default one.
		/// </summary>
		/// <typeparam name="TManager">
		/// The type of the manager that is used to manage the webhook subscriptions.
		/// </typeparam>
		/// <param name="lifetime">
		/// The service lifetime of the manager to be registered.
		/// </param>
		/// <returns>
		/// Returns this instance of the <see cref="WebhookSubscriptionBuilder{TSubscription,TKey}"/>.
		/// </returns>
		public WebhookSubscriptionBuilder<TSubscription, TKey> UseSubscriptionManager<TManager>(ServiceLifetime lifetime = ServiceLifetime.Scoped)
			where TManager : WebhookSubscriptionManager<TSubscription, TKey> {

			Services.RemoveAll<EntityManager<TSubscription>>();
			Services.RemoveAll<WebhookSubscriptionManager<TSubscription, TKey>>();

			Services.TryAdd(new ServiceDescriptor(typeof(WebhookSubscriptionManager<TSubscription, TKey>), typeof(TManager), lifetime));

			if (typeof(TManager) != typeof(WebhookSubscriptionManager<TSubscription, TKey>))
				Services.Add(new ServiceDescriptor(typeof(TManager), typeof(TManager), lifetime));

			return this;
		}

		/// <summary>
		/// Registers the default <see cref="WebhookSubscriptionManager{TSubscription,TKey}"/>
		/// </summary>
		/// <returns>
		/// Returns this instance of the <see cref="WebhookSubscriptionBuilder{TSubscription,TKey}"/>.
		/// </returns>
		public WebhookSubscriptionBuilder<TSubscription,TKey> UseSubscriptionManager()
			=> UseSubscriptionManager<WebhookSubscriptionManager<TSubscription, TKey>>();

		/// <summary>
		/// Adds a validator of webhook subscriptions.
		/// </summary>
		/// <typeparam name="TValidator">
		/// The type of the validator that is used to validate the webhook subscriptions.
		/// </typeparam>
		/// <param name="lifetime">
		/// The service lifetime of the validator to be registered.
		/// </param>
		/// <returns>
		/// Returns this instance of the <see cref="WebhookSubscriptionBuilder{TSubscription,TKey}"/>.
		/// </returns>
		public WebhookSubscriptionBuilder<TSubscription, TKey> AddSubscriptionValidator<TValidator>(ServiceLifetime lifetime = ServiceLifetime.Singleton)
			where TValidator : class, IWebhookSubscriptionValidator<TSubscription, TKey> {

			Services.AddEntityValidator<TValidator>(lifetime);

			return this;
		}
	}
}
