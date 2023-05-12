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

using System.Diagnostics.CodeAnalysis;

using Deveel.Webhooks.SendGrid;

namespace Deveel.Webhooks {
	/// <summary>
	/// Extends the <see cref="WebhookReceiverBuilder{TWebhook}"/> to add the features to receive
	/// webhooks from SendGrid.
	/// </summary>
	[ExcludeFromCodeCoverage]
	public static class WebhookReceiverBuilderExtensions {
		/// <summary>
		/// Adds a handler to the receiver to handle the incoming webhooks from SendGrid.
		/// </summary>
		/// <typeparam name="THandler">
		/// The type of the handler to add to the receiver.
		/// </typeparam>
		/// <param name="builder">
		/// The builder to add the handler to.
		/// </param>
		/// <returns>
		/// Returns the same instance of the <paramref name="builder"/> to allow chaining.
		/// </returns>
		public static WebhookReceiverBuilder<SendGridWebhook> HandleSendGridWebhook<THandler>(this WebhookReceiverBuilder<SendGridWebhook> builder)
			where THandler : class, IWebhookHandler<SendGridWebhook>
			=> builder.AddHandler<THandler>();

		/// <summary>
		/// Adds a handler to the receiver to handle the incoming webhooks from SendGrid.
		/// </summary>
		/// <param name="builder">
		/// The builder to add the handler to.
		/// </param>
		/// <param name="handler">
		/// The delegate to handle the incoming webhook.
		/// </param>
		/// <returns>
		/// Returns the same instance of the <paramref name="builder"/> to allow chaining.
		/// </returns>
		public static WebhookReceiverBuilder<SendGridWebhook> HandleSendGrid(this WebhookReceiverBuilder<SendGridWebhook> builder, Func<SendGridWebhook, Task> handler)
			=> builder.HandleAsync(handler);

		/// <summary>
		/// Adds a handler to the receiver to handle the incoming webhooks from SendGrid.
		/// </summary>
		/// <typeparam name="T1">The type of the first argument of the handler</typeparam>
		/// <param name="builder">
		/// The builder to add the handler to.
		/// </param>
		/// <param name="handler">
		/// The delegate to handle the incoming webhook.
		/// </param>
		/// <returns>
		/// Returns the same instance of the <paramref name="builder"/> to allow chaining.
		/// </returns>
		public static WebhookReceiverBuilder<SendGridWebhook> HandleSendGrid<T1>(this WebhookReceiverBuilder<SendGridWebhook> builder, Func<SendGridWebhook, T1, Task> handler)
			=> builder.HandleAsync(handler);

		/// <summary>
		/// Adds a handler to the receiver to handle the incoming webhooks from SendGrid.
		/// </summary>
		/// <typeparam name="T1">The type of the first argument of the handler</typeparam>
		/// <typeparam name="T2">The type of the second argument of the handler</typeparam>
		/// <param name="builder">
		/// The builder to add the handler to.
		/// </param>
		/// <param name="handler">
		/// The delegate to handle the incoming webhook.
		/// </param>
		/// <returns>
		/// Returns the same instance of the <paramref name="builder"/> to allow chaining.
		/// </returns>
		public static WebhookReceiverBuilder<SendGridWebhook> HandleSendGrid<T1, T2>(this WebhookReceiverBuilder<SendGridWebhook> builder, Func<SendGridWebhook, T1, T2, Task> handler)
			=> builder.HandleAsync(handler);

		/// <summary>
		/// Adds a handler to the receiver to handle the incoming webhooks from SendGrid.
		/// </summary>
		/// <typeparam name="T1">The type of the first argument of the handler</typeparam>
		/// <typeparam name="T2">The type of the second argument of the handler</typeparam>
		/// <typeparam name="T3">The type of the third argument of the handler</typeparam>
		/// <param name="builder">
		/// The builder to add the handler to.
		/// </param>
		/// <param name="handler">
		/// The delegate to handle the incoming webhook.
		/// </param>
		/// <returns>
		/// Returns the same instance of the <paramref name="builder"/> to allow chaining.
		/// </returns>
		public static WebhookReceiverBuilder<SendGridWebhook> HandleSendGrid<T1, T2, T3>(this WebhookReceiverBuilder<SendGridWebhook> builder, Func<SendGridWebhook, T1, T2, T3, Task> handler)
			=> builder.HandleAsync(handler);

		/// <summary>
		/// Adds a handler to the receiver to handle the incoming webhooks from SendGrid.
		/// </summary>
		/// <param name="builder">
		/// The builder to add the handler to.
		/// </param>
		/// <param name="handler">
		/// The delegate to handle the incoming webhook.
		/// </param>
		/// <returns>
		/// Returns the same instance of the <paramref name="builder"/> to allow chaining.
		/// </returns>
		public static WebhookReceiverBuilder<SendGridWebhook> HandleSendGrid(this WebhookReceiverBuilder<SendGridWebhook> builder, Action<SendGridWebhook> handler)
			=> builder.Handle(handler);

		/// <summary>
		/// Adds a handler to the receiver to handle the incoming webhooks from SendGrid.
		/// </summary>
		/// <typeparam name="T1">The type of the first argument of the handler</typeparam>
		/// <param name="builder">
		/// The builder to add the handler to.
		/// </param>
		/// <param name="handler">
		/// The delegate to handle the incoming webhook.
		/// </param>
		/// <returns>
		/// Returns the same instance of the <paramref name="builder"/> to allow chaining.
		/// </returns>
		public static WebhookReceiverBuilder<SendGridWebhook> HandleSendGrid<T1>(this WebhookReceiverBuilder<SendGridWebhook> builder, Action<SendGridWebhook, T1> handler)
			=> builder.Handle(handler);

		/// <summary>
		/// Adds a handler to the receiver to handle the incoming webhooks from SendGrid.
		/// </summary>
		/// <typeparam name="T1">The type of the first argument of the handler</typeparam>
		/// <typeparam name="T2">The type of the second argument of the handler</typeparam>
		/// <param name="builder">
		/// The builder to add the handler to.
		/// </param>
		/// <param name="handler">
		/// The delegate to handle the incoming webhook.
		/// </param>
		/// <returns>
		/// Returns the same instance of the <paramref name="builder"/> to allow chaining.
		/// </returns>
		public static WebhookReceiverBuilder<SendGridWebhook> HandleSendGrid<T1, T2>(this WebhookReceiverBuilder<SendGridWebhook> builder, Action<SendGridWebhook, T1, T2> handler)
			=> builder.Handle(handler);

		/// <summary>
		/// Adds a handler to the receiver to handle the incoming webhooks from SendGrid.
		/// </summary>
		/// <typeparam name="T1">The type of the first argument of the handler</typeparam>
		/// <typeparam name="T2">The type of the second argument of the handler</typeparam>
		/// <typeparam name="T3">The type of the third argument of the handler</typeparam>
		/// <param name="builder">
		/// The builder to add the handler to.
		/// </param>
		/// <param name="handler">
		/// The delegate to handle the incoming webhook.
		/// </param>
		/// <returns>
		/// Returns the same instance of the <paramref name="builder"/> to allow chaining.
		/// </returns>
		public static WebhookReceiverBuilder<SendGridWebhook> HandleSendGrid<T1, T2, T3>(this WebhookReceiverBuilder<SendGridWebhook> builder, Action<SendGridWebhook, T1, T2, T3> handler)
			=> builder.Handle(handler);


		//-- Emails

		public static WebhookReceiverBuilder<SendGridEmail> HandleSendGridEmail<THandler>(this WebhookReceiverBuilder<SendGridEmail> builder)
			where THandler : class, IWebhookHandler<SendGridEmail>
			=> builder.AddHandler<THandler>();

		public static WebhookReceiverBuilder<SendGridEmail> HandleSendGridEmail(this WebhookReceiverBuilder<SendGridEmail> builder, Func<SendGridEmail, Task> handler)
			=> builder.HandleAsync(handler);

		public static WebhookReceiverBuilder<SendGridEmail> HandleSendGridEmail<T1>(this WebhookReceiverBuilder<SendGridEmail> builder, Func<SendGridEmail, T1, Task> handler)
			=> builder.HandleAsync(handler);

		public static WebhookReceiverBuilder<SendGridEmail> HandleSendGridEmail<T1, T2>(this WebhookReceiverBuilder<SendGridEmail> builder, Func<SendGridEmail, T1, T2, Task> handler)
			=> builder.HandleAsync(handler);

		public static WebhookReceiverBuilder<SendGridEmail> HandleSendGridEmail<T1, T2, T3>(this WebhookReceiverBuilder<SendGridEmail> builder, Func<SendGridEmail, T1, T2, T3, Task> handler)
			=> builder.HandleAsync(handler);

		public static WebhookReceiverBuilder<SendGridEmail> HandleSendGridEmail(this WebhookReceiverBuilder<SendGridEmail> builder, Action<SendGridEmail> handler)
			=> builder.Handle(handler);

		public static WebhookReceiverBuilder<SendGridEmail> HandleSendGridEmail<T1>(this WebhookReceiverBuilder<SendGridEmail> builder, Action<SendGridEmail, T1> handler)
			=> builder.Handle(handler);

		public static WebhookReceiverBuilder<SendGridEmail> HandleSendGridEmail<T1, T2>(this WebhookReceiverBuilder<SendGridEmail> builder, Action<SendGridEmail, T1, T2> handler)
			=> builder.Handle(handler);

		public static WebhookReceiverBuilder<SendGridEmail> HandleSendGridEmail<T1, T2, T3>(this WebhookReceiverBuilder<SendGridEmail> builder, Action<SendGridEmail, T1, T2, T3> handler)
			=> builder.Handle(handler);
	}
}
