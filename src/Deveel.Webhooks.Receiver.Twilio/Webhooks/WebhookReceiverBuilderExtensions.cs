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
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Deveel.Webhooks.Twilio;

namespace Deveel.Webhooks {
	/// <summary>
	/// Extends the <see cref="WebhookReceiverBuilder{TwilioWebhook}"/> to add support for handling
	/// Twilio webhooks.
	/// </summary>
	[ExcludeFromCodeCoverage]
	public static class WebhookReceiverBuilderExtensions {
		/// <summary>
		/// Adds a handler for Twilio webhooks.
		/// </summary>
		/// <typeparam name="THandler">
		/// The type of handler that supports Twilio webhooks.
		/// </typeparam>
		/// <param name="builder">
		/// The instance of the <see cref="WebhookReceiverBuilder{TwilioWebhook}"/> to add the handler to.
		/// </param>
		/// <returns>
		/// Returns the same instance of the <see cref="WebhookReceiverBuilder{TwilioWebhook}"/> to allow
		/// further chaining of the builder.
		/// </returns>
		public static WebhookReceiverBuilder<TwilioWebhook> HandleTwilio<THandler>(this WebhookReceiverBuilder<TwilioWebhook> builder)
			where THandler : class, IWebhookHandler<TwilioWebhook>
			=> builder.AddHandler<THandler>();

		/// <summary>
		/// Adds a delegated handler for Twilio webhooks.
		/// </summary>
		/// <param name="builder">
		/// The instance of the <see cref="WebhookReceiverBuilder{TwilioWebhook}"/> to add the handler to.
		/// </param>
		/// <param name="handler">
		/// The delegate that will handle a Twilio webhook when received.
		/// </param>
		/// <returns>
		/// Returns the same instance of the <see cref="WebhookReceiverBuilder{TwilioWebhook}"/> to allow
		/// further chaining of the builder.
		/// </returns>
		public static WebhookReceiverBuilder<TwilioWebhook> HandleTwilio(this WebhookReceiverBuilder<TwilioWebhook> builder, Func<TwilioWebhook, Task> handler)
			=> builder.HandleAsync(handler);

		/// <summary>
		/// Adds a delegated handler for Twilio webhooks.
		/// </summary>
		/// <typeparam name="T1">
		/// The type of the first parameter of the delegate.
		/// </typeparam>
		/// <param name="builder">
		/// The instance of the <see cref="WebhookReceiverBuilder{TwilioWebhook}"/> to add the handler to.
		/// </param>
		/// <param name="handler">
		/// The delegate that will handle a Twilio webhook when received.
		/// </param>
		/// <returns>
		/// Returns the same instance of the <see cref="WebhookReceiverBuilder{TwilioWebhook}"/> to allow
		/// further chaining of the builder.
		/// </returns>
		public static WebhookReceiverBuilder<TwilioWebhook> HandleTwilio<T1>(this WebhookReceiverBuilder<TwilioWebhook> builder, Func<TwilioWebhook, T1, Task> handler)
			=> builder.HandleAsync(handler);

		/// <summary>
		/// Adds a delegated handler for Twilio webhooks.
		/// </summary>
		/// <typeparam name="T1">
		/// The type of the first parameter of the delegate.
		/// </typeparam>
		/// <typeparam name="T2">
		/// The type of the second parameter of the delegate.
		/// </typeparam>
		/// <param name="builder">
		/// The instance of the <see cref="WebhookReceiverBuilder{TwilioWebhook}"/> to add the handler to.
		/// </param>
		/// <param name="handler">
		/// The delegate that will handle a Twilio webhook when received.
		/// </param>
		/// <returns>
		/// Returns the same instance of the <see cref="WebhookReceiverBuilder{TwilioWebhook}"/> to allow
		/// further chaining of the builder.
		/// </returns>
		public static WebhookReceiverBuilder<TwilioWebhook> HandleTwilio<T1, T2>(this WebhookReceiverBuilder<TwilioWebhook> builder, Func<TwilioWebhook, T1, T2, Task> handler)
			=> builder.HandleAsync(handler);

		/// <summary>
		/// Adds a delegated handler for Twilio webhooks.
		/// </summary>
		/// <typeparam name="T1">
		/// The type of the first parameter of the delegate.
		/// </typeparam>
		/// <typeparam name="T2">
		/// The type of the second parameter of the delegate.
		/// </typeparam>
		/// <typeparam name="T3">
		/// The type of the third parameter of the delegate.
		/// </typeparam>
		/// <param name="builder">
		/// The instance of the <see cref="WebhookReceiverBuilder{TwilioWebhook}"/> to add the handler to.
		/// </param>
		/// <param name="handler">
		/// The delegate that will handle a Twilio webhook when received.
		/// </param>
		/// <returns>
		/// Returns the same instance of the <see cref="WebhookReceiverBuilder{TwilioWebhook}"/> to allow
		/// further chaining of the builder.
		/// </returns>
		public static WebhookReceiverBuilder<TwilioWebhook> HandleTwilio<T1, T2, T3>(this WebhookReceiverBuilder<TwilioWebhook> builder, Func<TwilioWebhook, T1, T2, T3, Task> handler)
			=> builder.HandleAsync(handler);

		/// <summary>
		/// Adds a delegated handler for Twilio webhooks.
		/// </summary>
		/// <param name="builder">
		/// The instance of the <see cref="WebhookReceiverBuilder{TwilioWebhook}"/> to add the handler to.
		/// </param>
		/// <param name="handler">
		/// The delegate that will handle a Twilio webhook when received.
		/// </param>
		/// <returns>
		/// Returns the same instance of the <see cref="WebhookReceiverBuilder{TwilioWebhook}"/> to allow
		/// further chaining of the builder.
		/// </returns>
		public static WebhookReceiverBuilder<TwilioWebhook> HandleTwilio(this WebhookReceiverBuilder<TwilioWebhook> builder, Action<TwilioWebhook> handler)
			=> builder.Handle(handler);

		/// <summary>
		/// Adds a delegated handler for Twilio webhooks.
		/// </summary>
		/// <typeparam name="T1">
		/// The type of the first parameter of the delegate.
		/// </typeparam>
		/// <param name="builder">
		/// The instance of the <see cref="WebhookReceiverBuilder{TwilioWebhook}"/> to add the handler to.
		/// </param>
		/// <param name="handler">
		/// The delegate that will handle a Twilio webhook when received.
		/// </param>
		/// <returns>
		/// Returns the same instance of the <see cref="WebhookReceiverBuilder{TwilioWebhook}"/> to allow
		/// further chaining of the builder.
		/// </returns>
		public static WebhookReceiverBuilder<TwilioWebhook> HandleTwilio<T1>(this WebhookReceiverBuilder<TwilioWebhook> builder, Action<TwilioWebhook, T1> handler)
			=> builder.Handle(handler);

		/// <summary>
		/// Adds a delegated handler for Twilio webhooks.
		/// </summary>
		/// <typeparam name="T1">
		/// The type of the first parameter of the delegate.
		/// </typeparam>
		/// <typeparam name="T2">
		/// The type of the second parameter of the delegate.
		/// </typeparam>
		/// <param name="builder">
		/// The instance of the <see cref="WebhookReceiverBuilder{TwilioWebhook}"/> to add the handler to.
		/// </param>
		/// <param name="handler">
		/// The delegate that will handle a Twilio webhook when received.
		/// </param>
		/// <returns>
		/// Returns the same instance of the <see cref="WebhookReceiverBuilder{TwilioWebhook}"/> to allow
		/// further chaining of the builder.
		/// </returns>
		public static WebhookReceiverBuilder<TwilioWebhook> HandleTwilio<T1, T2>(this WebhookReceiverBuilder<TwilioWebhook> builder, Action<TwilioWebhook, T1, T2> handler)
			=> builder.Handle(handler);

		/// <summary>
		/// Adds a delegated handler for Twilio webhooks.
		/// </summary>
		/// <typeparam name="T1">
		/// The type of the first parameter of the delegate.
		/// </typeparam>
		/// <typeparam name="T2">
		/// The type of the second parameter of the delegate.
		/// </typeparam>
		/// <typeparam name="T3">
		/// The type of the third parameter of the delegate.
		/// </typeparam>
		/// <param name="builder">
		/// The instance of the <see cref="WebhookReceiverBuilder{TwilioWebhook}"/> to add the handler to.
		/// </param>
		/// <param name="handler">
		/// The delegate that will handle a Twilio webhook when received.
		/// </param>
		/// <returns>
		/// Returns the same instance of the <see cref="WebhookReceiverBuilder{TwilioWebhook}"/> to allow
		/// further chaining of the builder.
		/// </returns>
		public static WebhookReceiverBuilder<TwilioWebhook> HandleTwilio<T1, T2, T3>(this WebhookReceiverBuilder<TwilioWebhook> builder, Action<TwilioWebhook, T1, T2, T3> handler)
			=> builder.Handle(handler);
	}
}
