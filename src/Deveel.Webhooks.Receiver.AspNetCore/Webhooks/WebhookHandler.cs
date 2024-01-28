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

namespace Deveel.Webhooks {
	/// <summary>
	/// Provides a static factory to create a <see cref="IWebhookHandler{TWebhook}"/> from a delegate.
	/// </summary>
	public static class WebhookHandler {
		/// <summary>
		/// Creates a new instance of <see cref="IWebhookHandler{TWebhook}"/> from the given delegate.
		/// </summary>
		/// <typeparam name="TWebhook">
		/// The type of webhook that is handled by the handler
		/// </typeparam>
		/// <param name="handler">
		/// The delegate that is invoked when the handler is executed
		/// </param>
		/// <returns>
		/// Returns an instance of <see cref="IWebhookHandler{TWebhook}"/> that wraps the given delegate.
		/// </returns>
		/// <exception cref="ArgumentNullException">
		/// Thrown if the given delegate is <c>null</c>.
		/// </exception>
		/// <exception cref="ArgumentException">
		/// Thrown if the first argument of the delegate is not of type <typeparamref name="TWebhook"/>.
		/// </exception>
		public static IWebhookHandler<TWebhook> Create<TWebhook>(Delegate handler)
			where TWebhook : class
			=> new DelegatedWebhookHandler<TWebhook>(handler);

		/// <summary>
		/// Creates a new instance of <see cref="IWebhookHandler{TWebhook}"/> from the given delegate,
		/// initializing it with the given services.
		/// </summary>
		/// <typeparam name="TWebhook">
		/// The type of webhook that is handled by the handler
		/// </typeparam>
		/// <param name="handler">
		/// The delegate that is invoked when the handler is executed
		/// </param>
		/// <param name="services">
		/// The service provider used to resolve the dependencies of the delegate.
		/// </param>
		/// <returns>
		/// Returns an instance of <see cref="IWebhookHandler{TWebhook}"/> that wraps the given delegate.
		/// </returns>
		/// <exception cref="ArgumentNullException">
		/// Thrown if the given delegate is <c>null</c>.
		/// </exception>
		/// <exception cref="ArgumentException">
		/// Thrown if the first argument of the delegate is not of type <typeparamref name="TWebhook"/>.
		/// </exception>
		public static IWebhookHandler<TWebhook> Create<TWebhook>(Delegate handler, IServiceProvider services)
			where TWebhook : class {
			var delegated = new DelegatedWebhookHandler<TWebhook>(handler);
			delegated.Initialize(services);
			return delegated;
		}
	}
}
