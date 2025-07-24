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
	/// A builder used to configure the webhook notification service.
	/// </summary>
	/// <typeparam name="TWebhook">
	/// The type of the webhook to notify.
	/// </typeparam>
	public sealed class WebhookNotifierBuilder<TWebhook> where TWebhook : class {
		/// <summary>
		/// Constructs the builder with the given service collection.
		/// </summary>
		/// <param name="services">
		/// The service collection to add the builder to.
		/// </param>
		/// <remarks>
		/// When the builder is constructed, a default set of services is registered
		/// to provide the best effort to make a notifier work.
		/// </remarks>
		/// <exception cref="ArgumentNullException">
		/// Thrown when the given <paramref name="services"/> is <c>null</c>.
		/// </exception>
		public WebhookNotifierBuilder(IServiceCollection services) {
			Services = services ?? throw new ArgumentNullException(nameof(services));

			RegisterDefaultServices();
		}

		/// <summary>
		/// Gets the service collection that is used to build the notifier.
		/// </summary>
		public IServiceCollection Services { get; }

		private void RegisterDefaultServices() {
			Services.TryAddScoped<IWebhookNotifier<TWebhook>, WebhookNotifier<TWebhook>>();
			Services.TryAddScoped<WebhookNotifier<TWebhook>>();
			Services.TryAddScoped<IWebhookSender<TWebhook>, WebhookSender<TWebhook>>();
			Services.TryAddScoped<WebhookSender<TWebhook>>();

			if (typeof(Webhook).IsAssignableFrom(typeof(TWebhook))) {
				var factoryType = typeof(DefaultWebhookFactory<>).MakeGenericType(typeof(TWebhook));
				Services.TryAddSingleton(typeof(IWebhookFactory<TWebhook>), factoryType);
				Services.TryAddSingleton(factoryType);
			}

			// TODO: register the default filter evaluator
		}

		/// <summary>
		/// Adds a sender service for the notifier.
		/// </summary>
		/// <param name="configure">
		/// A callback to configure the sender.
		/// </param>
		/// <returns>
		/// Returns an instance of the builder to allow chaining.
		/// </returns>
		public WebhookNotifierBuilder<TWebhook> UseSender(Action<WebhookSenderBuilder<TWebhook>> configure) {
			var builder = Services.AddWebhookSender<TWebhook>();
			configure?.Invoke(builder);

			return this;
		}

		/// <summary>
		/// Adds a sender service for the notifier.
		/// </summary>
		/// <param name="configure">
		/// A function used to configure the sender.
		/// </param>
		/// <returns>
		/// Returns an instance of the builder to allow chaining.
		/// </returns>
		public WebhookNotifierBuilder<TWebhook> UseSender(Action<WebhookSenderOptions<TWebhook>> configure) {
			Services.AddWebhookSender<TWebhook>(configure);

			return this;
		}

		/// <summary>
		/// Adds the default sender service for the notifier.
		/// </summary>
		/// <returns>
		/// Returns an instance of the builder to allow chaining.
		/// </returns>
		public WebhookNotifierBuilder<TWebhook> UseSender() 
			=> UseSender((WebhookSenderBuilder<TWebhook> builder) => { });

		/// <summary>
		/// Registers a notifier service to use.
		/// </summary>
		/// <typeparam name="TNotifier">
		/// The type of the notifier to use.
		/// </typeparam>
		/// <param name="lifetime">
		/// An optional value that specifies the lifetime of the service (by default
		/// set to <see cref="ServiceLifetime.Scoped"/>).
		/// </param>
		/// <returns>
		/// Returns an instance of the builder to allow chaining.
		/// </returns>
		public WebhookNotifierBuilder<TWebhook> UseNotifier<TNotifier>(ServiceLifetime lifetime = ServiceLifetime.Scoped)
			where TNotifier : class, IWebhookNotifier<TWebhook> {

			Services.RemoveAll<IWebhookNotifier<TWebhook>>();

			Services.Add(new ServiceDescriptor(typeof(IWebhookNotifier<TWebhook>), typeof(TNotifier), lifetime));
			Services.TryAdd(new ServiceDescriptor(typeof(TNotifier), typeof(TNotifier), lifetime));

			return this;
		}

		/// <summary>
		/// Registers the default notifier service to use.
		/// </summary>
		/// <returns>
		/// Returns an instance of the builder to allow chaining.
		/// </returns>
		public WebhookNotifierBuilder<TWebhook> UseNotifier()
			=> UseNotifier<WebhookNotifier<TWebhook>>();


		/// <summary>
		/// Registers a factory service to use to create the webhook.
		/// </summary>
		/// <typeparam name="TFactory">
		/// The type of the factory to use.
		/// </typeparam>
		/// <param name="lifetime">
		/// An optional value that specifies the lifetime of the service (by default
		/// set to <see cref="ServiceLifetime.Singleton"/>).
		/// </param>
		/// <returns></returns>
		public WebhookNotifierBuilder<TWebhook> UseWebhookFactory<TFactory>(ServiceLifetime lifetime = ServiceLifetime.Singleton)
			where TFactory : class, IWebhookFactory<TWebhook> {

			Services.RemoveAll<IWebhookFactory<TWebhook>>();

			Services.TryAdd(new ServiceDescriptor(typeof(IWebhookFactory<TWebhook>), typeof(TFactory), lifetime));
			Services.Add(new ServiceDescriptor(typeof(TFactory), typeof(TFactory), lifetime));

			return this;
		}

		public WebhookNotifierBuilder<TWebhook> UseWebhookFactory() {
			if (!typeof(TWebhook).IsClass || typeof(TWebhook).IsAbstract)
				throw new InvalidOperationException("The webhook type must be a concrete class");

			// TODO: check if the TWebhook has a parameterless constructor
			if (!(typeof(TWebhook).GetConstructor(Type.EmptyTypes)?.IsPublic ?? false))
				throw new InvalidOperationException("The webhook type must have a public parameterless constructor");

			Services.RemoveAll<IWebhookFactory<TWebhook>>();

			var factoryType = typeof(DefaultWebhookFactory<>).MakeGenericType(typeof(TWebhook));
			Services.AddSingleton(typeof(IWebhookFactory<TWebhook>), factoryType);
			Services.AddSingleton(factoryType);

			if (typeof(TWebhook).IsAssignableFrom(typeof(Webhook))) {
				Services.AddSingleton(typeof(DefaultWebhookFactory));
			}

			return this;
		}

		public WebhookNotifierBuilder<TWebhook> UseWebhookFactory(Action<WebhookFactoryOptions<TWebhook>> configure) {
			UseWebhookFactory();
			if (configure != null) {
				Services.AddOptions<WebhookFactoryOptions<TWebhook>>()
					.Configure(configure);
			}

			return this;
		}

		/// <summary>
		/// Adds a service that evaluates webhooks against a set
		/// of filters, to determine whether they should be sent.
		/// </summary>
		/// <typeparam name="TEvaluator">
		/// The type of the evaluator to register.
		/// </typeparam>
		/// <param name="lifetime">
		/// An optional value that specifies the lifetime of the service (by default
		/// set to <see cref="ServiceLifetime.Singleton"/>).
		/// </param>
		/// <returns>
		/// Returns an instance of the builder to allow chaining.
		/// </returns>
		public WebhookNotifierBuilder<TWebhook> AddFilterEvaluator<TEvaluator>(ServiceLifetime lifetime = ServiceLifetime.Singleton)
			where TEvaluator : class, IWebhookFilterEvaluator<TWebhook> {

			Services.Add(new ServiceDescriptor(typeof(IWebhookFilterEvaluator<TWebhook>), typeof(TEvaluator), lifetime));
			Services.TryAdd(new ServiceDescriptor(typeof(TEvaluator), typeof(TEvaluator), lifetime));

			return this;
		}

		/// <summary>
		/// Registers a service that resolves the subscriptions to the
		/// notification of events.
		/// </summary>
		/// <param name="resolverType">
		/// The type of the resolver to register.
		/// </param>
		/// <param name="lifetime">
		/// An optional value that specifies the lifetime of the service (by default
		/// set to <see cref="ServiceLifetime.Scoped"/>).
		/// </param>
		/// <returns>
		/// Returns an instance of the builder to allow chaining.
		/// </returns>
		public WebhookNotifierBuilder<TWebhook> UseSubscriptionResolver(Type resolverType, ServiceLifetime lifetime = ServiceLifetime.Scoped) {
			if (typeof(IWebhookSubscriptionResolver<TWebhook>).IsAssignableFrom(resolverType)) {
				Services.Add(new ServiceDescriptor(typeof(IWebhookSubscriptionResolver<TWebhook>), resolverType, lifetime));
			} else {
				Func<IServiceProvider, IWebhookSubscriptionResolver<TWebhook>> factory = provider => {
					var resolver = (IWebhookSubscriptionResolver)provider.GetRequiredService(resolverType);
					return new WebhookSubscriptionResolverAdapter(resolver);
				};
				Services.Add(new ServiceDescriptor(typeof(IWebhookSubscriptionResolver<TWebhook>), factory, lifetime));
			}

			Services.TryAdd(new ServiceDescriptor(resolverType, resolverType, lifetime));
			return this;
		}

		/// <summary>
		/// Registers a service that resolves the subscriptions to the
		/// notification of events.
		/// </summary>
		/// <typeparam name="TResolver">
		/// The type of the resolver to register.
		/// </typeparam>
		/// <param name="lifetime">
		/// An optional value that specifies the lifetime of the service (by default
		/// set to <see cref="ServiceLifetime.Scoped"/>).
		/// </param>
		/// <returns></returns>
		public WebhookNotifierBuilder<TWebhook> UseSubscriptionResolver<TResolver>(ServiceLifetime lifetime = ServiceLifetime.Scoped)
			where TResolver : class, IWebhookSubscriptionResolver
			=> UseSubscriptionResolver(typeof(TResolver), lifetime);


		/// <summary>
		/// Adds a service that logs the delivery results of webhooks.
		/// </summary>
		/// <typeparam name="TLogger">
		/// The type of the logger to register.
		/// </typeparam>
		/// <param name="lifetime">
		/// An optional value that specifies the lifetime of the service (by default
		/// set to <see cref="ServiceLifetime.Scoped"/>).
		/// </param>
		/// <returns></returns>
		public WebhookNotifierBuilder<TWebhook> AddDeliveryLogger<TLogger>(ServiceLifetime lifetime = ServiceLifetime.Scoped)
			where TLogger : class, IWebhookDeliveryResultLogger<TWebhook> {

			Services.Add(new ServiceDescriptor(typeof(IWebhookDeliveryResultLogger<TWebhook>), typeof(TLogger), lifetime));
			Services.TryAdd(new ServiceDescriptor(typeof(TLogger), typeof(TLogger), lifetime));

			return this;
		}

		class WebhookSubscriptionResolverAdapter : IWebhookSubscriptionResolver<TWebhook> {
			private readonly IWebhookSubscriptionResolver _resolver;

			public WebhookSubscriptionResolverAdapter(IWebhookSubscriptionResolver resolver) {
				_resolver = resolver;
			}

			public Task<IList<IWebhookSubscription>> ResolveSubscriptionsAsync(string eventType, bool activeOnly, CancellationToken cancellationToken)
				=> _resolver.ResolveSubscriptionsAsync(eventType, activeOnly, cancellationToken);
		}

	}
}
