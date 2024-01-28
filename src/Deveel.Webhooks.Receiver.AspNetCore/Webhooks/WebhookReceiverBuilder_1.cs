// Copyright 2022-2024 Antonello Provenzano
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

			RegisterDefaultServices();
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

		private void RegisterDefaultServices() {
			Services.TryAddTransient<IWebhookReceiver<TWebhook>, WebhookReceiver<TWebhook>>();
			Services.TryAddTransient<WebhookReceiver<TWebhook>>();
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
		public WebhookReceiverBuilder<TWebhook> UseReceiver<TReceiver>(ServiceLifetime lifetime = ServiceLifetime.Transient)
			where TReceiver : class, IWebhookReceiver<TWebhook> {

			Services.Add(new ServiceDescriptor(typeof(TReceiver), typeof(TReceiver), lifetime));

			if (!typeof(TReceiver).IsAbstract)
				Services.Add(new ServiceDescriptor(typeof(TReceiver), typeof(TReceiver), lifetime));

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
		public WebhookReceiverBuilder<TWebhook> AddHandler<THandler>(ServiceLifetime lifetime = ServiceLifetime.Scoped) 
			where THandler : class, IWebhookHandler<TWebhook> {

			Services.Add(new ServiceDescriptor(typeof(IWebhookHandler<TWebhook>), typeof(THandler), lifetime));

			if (typeof(THandler).IsClass && !typeof(THandler).IsAbstract)
				Services.Add(new ServiceDescriptor(typeof(THandler), typeof(THandler), lifetime));

			return this;
		}

		/// <summary>
		/// Registers an handler instance for the webhooks of type <typeparamref name="TWebhook"/>
		/// </summary>
		/// <typeparam name="THandler">
		/// The type of the handler to use for the webhooks of type <typeparamref name="TWebhook"/>
		/// </typeparam>
		/// <param name="handler">
		/// The instance of the handler to use for the webhooks of type <typeparamref name="TWebhook"/>
		/// </param>
		/// <returns>
		/// Returns the current builder instance with the handler registered
		/// </returns>
		public WebhookReceiverBuilder<TWebhook> AddHandler<THandler>(THandler handler)
			where THandler : class, IWebhookHandler<TWebhook> {

			Services.AddSingleton<THandler>(handler);

			return this;
		}

		private WebhookReceiverBuilder<TWebhook> HandleDelegate(Delegate handler) {
			Services.AddScoped<IWebhookHandler<TWebhook>>(sp => WebhookHandler.Create<TWebhook>(handler, sp));
			return this;
		}

		/// <summary>
		/// Registers the given delegate as handler for the webhooks of type <typeparamref name="TWebhook"/>
		/// </summary>
		/// <param name="handler">
		/// The delegate that should be used as handler for the webhooks.
		/// </param>
		/// <returns>
		/// Returns the current builder instance with the handler registered
		/// </returns>
		public WebhookReceiverBuilder<TWebhook> HandleAsync(Func<TWebhook, Task> handler)
			=> HandleDelegate(handler);

		/// <summary>
		/// Registers the given delegate as handler for the webhooks of type <typeparamref name="TWebhook"/>
		/// </summary>
		/// <typeparam name="T1">The type of the first argument of the handler</typeparam>
		/// <param name="handler">
		/// The delegate that should be used as handler for the webhooks.
		/// </param>
		/// <returns>
		/// Returns the current builder instance with the handler registered
		/// </returns>
		public WebhookReceiverBuilder<TWebhook> HandleAsync<T1>(Func<TWebhook, T1, Task> handler)
			=> HandleDelegate(handler);

		/// <summary>
		/// Registers the given delegate as handler for the webhooks of type <typeparamref name="TWebhook"/>
		/// </summary>
		/// <typeparam name="T1">The type of the first argument of the handler</typeparam>
		/// <typeparam name="T2">The type of the second argument of the handler</typeparam>
		/// <param name="handler">
		/// The delegate that should be used as handler for the webhooks.
		/// </param>
		/// <returns>
		/// Returns the current builder instance with the handler registered
		/// </returns>
		public WebhookReceiverBuilder<TWebhook> HandleAsync<T1, T2>(Func<TWebhook, T1, T2, Task> handler)
			=> HandleDelegate(handler);

		/// <summary>
		/// Registers the given delegate as handler for the webhooks of type <typeparamref name="TWebhook"/>
		/// </summary>
		/// <typeparam name="T1">The type of the first argument of the handler</typeparam>
		/// <typeparam name="T2">The type of the second argument of the handler</typeparam>
		/// <typeparam name="T3">The type of the third argument of the handler</typeparam>
		/// <param name="handler">
		/// The delegate that should be used as handler for the webhooks.
		/// </param>
		/// <returns>
		/// Returns the current builder instance with the handler registered
		/// </returns>
		public WebhookReceiverBuilder<TWebhook> HandleAsync<T1, T2, T3>(Func<TWebhook, T1, T2, T3, Task> handler) 
			=> HandleDelegate(handler);

		/// <summary>
		/// Registers the given delegate as handler for the webhooks of type <typeparamref name="TWebhook"/>
		/// </summary>
		/// <param name="handler">
		/// The delegate that should be used as handler for the webhooks.
		/// </param>
		/// <returns>
		/// Returns the current builder instance with the handler registered
		/// </returns>
		public WebhookReceiverBuilder<TWebhook> Handle(Action<TWebhook> handler)
			=> HandleDelegate(handler);

		/// <summary>
		/// Registers the given delegate as handler for the webhooks of type <typeparamref name="TWebhook"/>
		/// </summary>
		/// <typeparam name="T1">The type of the first argument of the handler</typeparam>
		/// <param name="handler">
		/// The delegate that should be used as handler for the webhooks.
		/// </param>
		/// <returns>
		/// Returns the current builder instance with the handler registered
		/// </returns>
		public WebhookReceiverBuilder<TWebhook> Handle<T1>(Action<TWebhook, T1> handler)
			=> HandleDelegate(handler);

		/// <summary>
		/// Registers the given delegate as handler for the webhooks of type <typeparamref name="TWebhook"/>
		/// </summary>
		/// <typeparam name="T1">The type of the first argument of the handler</typeparam>
		/// <typeparam name="T2">The type of the second argument of the handler</typeparam>
		/// <param name="handler">
		/// The delegate that should be used as handler for the webhooks.
		/// </param>
		/// <returns>
		/// Returns the current builder instance with the handler registered
		/// </returns>
		public WebhookReceiverBuilder<TWebhook> Handle<T1, T2>(Action<TWebhook, T1, T2> handler)
			=> Handle(handler);

		/// <summary>
		/// Registers the given delegate as handler for the webhooks of type <typeparamref name="TWebhook"/>
		/// </summary>
		/// <typeparam name="T1">The type of the first argument of the handler</typeparam>
		/// <typeparam name="T2">The type of the second argument of the handler</typeparam>
		/// <typeparam name="T3">The type of the third argument of the handler</typeparam>
		/// <param name="handler">
		/// The delegate that should be used as handler for the webhooks.
		/// </param>
		/// <returns>
		/// Returns the current builder instance with the handler registered
		/// </returns>
		public WebhookReceiverBuilder<TWebhook> Handle<T1, T2, T3>(Action<TWebhook, T1, T2, T3> handler) 
			=> Handle(handler);

	}
}
