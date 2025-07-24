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

using System.Diagnostics.CodeAnalysis;

using Deveel.Webhooks.SendGrid;

using Microsoft.AspNetCore.Builder;

namespace Deveel.Webhooks {
	/// <summary>
	/// Extends the <see cref="IApplicationBuilder"/> with methods
	/// to map a SendGrid webhooks and emails.
	/// </summary>
	[ExcludeFromCodeCoverage]
	public static class ApplicationBuilderExtensions {
		/// <summary>
		/// Maps a SendGrid webhook receiver to the given <paramref name="path"/>
		/// </summary>
		/// <param name="app">
		/// The instance of the <see cref="IApplicationBuilder"/> to extend
		/// </param>
		/// <param name="path">
		/// The path to map the webhook to (default is <c>/webhook/sendgrid</c>)
		/// </param>
		/// <remarks>
		/// This method will make the receiver to listen for incoming SendGrid
		/// webhooks at the given <paramref name="path"/> and invoke any registered
		/// handler for this kind of webhooks in the application.
		/// </remarks>
		/// <returns>
		/// Returns the instance of the <see cref="IApplicationBuilder"/> to extend
		/// </returns>
		public static IApplicationBuilder MapSendGridWebhook(this IApplicationBuilder app, string path = "/webhook/sendgrid")
			=> app.MapWebhook<SendGridWebhook>(path, new WebhookHandlingOptions {
				InvalidStatusCode = 400,
				// TODO: configure the options
			});

		/// <summary>
		/// Maps a SendGrid webhook handle to the given <paramref name="path"/>
		/// </summary>
		/// <param name="app">
		/// The instance of the <see cref="IApplicationBuilder"/> to extend
		/// </param>
		/// <param name="path">
		/// The path where to listen for incoming SendGrid webhooks
		/// </param>
		/// <param name="handler">
		/// The asynchronous handler to invoke when a webhook is received
		/// </param>
		/// <returns>
		/// Returns the instance of the <see cref="IApplicationBuilder"/> to extend
		/// </returns>
		public static IApplicationBuilder MapSendGridWebhook(this IApplicationBuilder app, string path, Func<SendGridWebhook, Task> handler)
			=> app.MapWebhook<SendGridWebhook>(path, handler);

		/// <summary>
		/// Maps a SendGrid webhook handler to the given <paramref name="path"/>
		/// </summary>
		/// <typeparam name="T1">
		/// The type of the first argument of the handler
		/// </typeparam>
		/// <param name="app">
		/// The instance of the <see cref="IApplicationBuilder"/> to extend
		/// </param>
		/// <param name="path">
		/// The path where to listen for incoming SendGrid webhooks
		/// </param>
		/// <param name="handler">
		/// The asynchronous handler to invoke when a webhook is received
		/// </param>
		/// <returns>
		/// Returns the instance of the <see cref="IApplicationBuilder"/> to extend
		/// </returns>
		public static IApplicationBuilder MapSendGridWebhook<T1>(this IApplicationBuilder app, string path, Func<SendGridWebhook, T1, Task> handler)
			=> app.MapWebhook<SendGridWebhook, T1>(path, handler);

		/// <summary>
		/// Maps a SendGrid webhook handler to the given <paramref name="path"/>
		/// </summary>
		/// <typeparam name="T1">
		/// The type of the first argument of the handler
		/// </typeparam>
		/// <typeparam name="T2">
		/// The type of the second argument of the handler
		/// </typeparam>
		/// <param name="app">
		/// The instance of the <see cref="IApplicationBuilder"/> to extend
		/// </param>
		/// <param name="path">
		/// The path where to listen for incoming SendGrid webhooks
		/// </param>
		/// <param name="handler">
		/// The asynchronous handler to invoke when a webhook is received
		/// </param>
		/// <returns>
		/// Returns the instance of the <see cref="IApplicationBuilder"/> to extend
		/// </returns>
		public static IApplicationBuilder MapSendGridWebhook<T1, T2>(this IApplicationBuilder app, string path, Func<SendGridWebhook, T1, T2, Task> handler)
			=> app.MapWebhook<SendGridWebhook, T1, T2>(path, handler);

		/// <summary>
		/// Maps a SendGrid webhook handler to the given <paramref name="path"/>
		/// </summary>
		/// <typeparam name="T1">
		/// The type of the first argument of the handler
		/// </typeparam>
		/// <typeparam name="T2">
		/// The type of the second argument of the handler
		/// </typeparam>
		/// <typeparam name="T3">
		/// The type of the third argument of the handler
		/// </typeparam>
		/// <param name="app">
		/// The instance of the <see cref="IApplicationBuilder"/> to extend
		/// </param>
		/// <param name="path">
		/// The path where to listen for incoming SendGrid webhooks
		/// </param>
		/// <param name="handler">
		/// The asynchronous handler to invoke when a webhook is received
		/// </param>
		/// <returns>
		/// Returns the instance of the <see cref="IApplicationBuilder"/> to extend
		/// </returns>
		public static IApplicationBuilder MapSendGridWebhook<T1, T2, T3>(this IApplicationBuilder app, string path, Func<SendGridWebhook, T1, T2, T3, Task> handler)
			=> app.MapWebhook<SendGridWebhook, T1, T2, T3>(path, handler);

		/// <summary>
		/// Maps a SendGrid webhook handler to the given <paramref name="path"/>
		/// </summary>
		/// <param name="app">
		/// The instance of the <see cref="IApplicationBuilder"/> to extend
		/// </param>
		/// <param name="path">
		/// The path where to listen for incoming SendGrid webhooks
		/// </param>
		/// <param name="handler">
		/// The handler function to invoke when a webhook is received
		/// </param>
		/// <returns>
		/// Returns the instance of the <see cref="IApplicationBuilder"/> to extend
		/// </returns>
		public static IApplicationBuilder MapSendGridWebhook(this IApplicationBuilder app, string path, Action<SendGridWebhook> handler)
			=> app.MapWebhook<SendGridWebhook>(path, handler);

		/// <summary>
		/// Maps a SendGrid webhook handler to the given <paramref name="path"/>
		/// </summary>
		/// <typeparam name="T1">
		/// The type of the first argument of the handler
		/// </typeparam>
		/// <param name="app">
		/// The instance of the <see cref="IApplicationBuilder"/> to extend
		/// </param>
		/// <param name="path">
		/// The path where to listen for incoming SendGrid webhooks
		/// </param>
		/// <param name="handler">
		/// The handler function to invoke when a webhook is received
		/// </param>
		/// <returns>
		/// Returns the instance of the <see cref="IApplicationBuilder"/> to extend
		/// </returns>
		public static IApplicationBuilder MapSendGridWebhook<T1>(this IApplicationBuilder app, string path, Action<SendGridWebhook, T1> handler)
			=> app.MapWebhook<SendGridWebhook, T1>(path, handler);

		/// <summary>
		/// Maps a SendGrid webhook handler to the given <paramref name="path"/>
		/// </summary>
		/// <typeparam name="T1">
		/// The type of the first argument of the handler
		/// </typeparam>
		/// <typeparam name="T2">
		/// The type of the second argument of the handler
		/// </typeparam>
		/// <param name="app">
		/// The instance of the <see cref="IApplicationBuilder"/> to extend
		/// </param>
		/// <param name="path">
		/// The path where to listen for incoming SendGrid webhooks
		/// </param>
		/// <param name="handler">
		/// The handler function to invoke when a webhook is received
		/// </param>
		/// <returns>
		/// Returns the instance of the <see cref="IApplicationBuilder"/> to extend
		/// </returns>
		public static IApplicationBuilder MapSendGridWebhook<T1, T2>(this IApplicationBuilder app, string path, Action<SendGridWebhook, T1, T2> handler)
			=> app.MapWebhook<SendGridWebhook, T1, T2>(path, handler);

		/// <summary>
		/// Maps a SendGrid webhook handler to the given <paramref name="path"/>
		/// </summary>
		/// <typeparam name="T1">
		/// The type of the first argument of the handler
		/// </typeparam>
		/// <typeparam name="T2">
		/// The type of the second argument of the handler
		/// </typeparam>
		/// <typeparam name="T3">
		/// The type of the third argument of the handler
		/// </typeparam>
		/// <param name="app">
		/// The instance of the <see cref="IApplicationBuilder"/> to extend
		/// </param>
		/// <param name="path">
		/// The path where to listen for incoming SendGrid webhooks
		/// </param>
		/// <param name="handler">
		/// The handler function to invoke when a webhook is received
		/// </param>
		/// <returns>
		/// Returns the instance of the <see cref="IApplicationBuilder"/> to extend
		/// </returns>
		public static IApplicationBuilder MapSendGridWebhook<T1, T2, T3>(this IApplicationBuilder app, string path, Action<SendGridWebhook, T1, T2, T3> handler)
			=> app.MapWebhook<SendGridWebhook, T1, T2, T3>(path, handler);

		/// <summary>
		/// Maps the SendGrid email receiver to the given <paramref name="path"/>
		/// </summary>
		/// <param name="app">
		/// The instance of the <see cref="IApplicationBuilder"/> to extend
		/// </param>
		/// <param name="path">
		/// The path where to listen for incoming SendGrid emails
		/// </param>
		/// <remarks>
		/// <para>
		/// By design, SendGrid emails are not considered as webhooks, since they implement
		/// other methodologies and designs, but instead they are non-signed HTTP requests: 
		/// this method provides a utility to exploit the infrastructure
		/// provided by the framework to listen and handle incoming SendGrid emails, as if
		/// they actually were webhooks.
		/// </para>
		/// <para>
		/// This method will make the receiver to listen for incoming SendGrid
		/// emails at the given <paramref name="path"/> and invoke any registered
		/// email handlers in the application.
		/// </para>
		/// </remarks>
		/// <returns>
		/// Returns the instance of the <see cref="IApplicationBuilder"/> to extend
		/// </returns>
		public static IApplicationBuilder MapSendGridEmail(this IApplicationBuilder app, string path = "/email/sendgrid")
			=> app.MapWebhook<SendGridEmail>(path, new WebhookHandlingOptions {
				InvalidStatusCode = 400,
				ResponseStatusCode = 201,
			});

		/// <summary>
		/// Maps a SendGrid email handler to the given <paramref name="path"/>
		/// </summary>
		/// <param name="app">
		/// The instance of the <see cref="IApplicationBuilder"/> to extend
		/// </param>
		/// <param name="path">
		/// The path where to listen for incoming SendGrid emails
		/// </param>
		/// <param name="handler">
		/// The asynchronous handler to invoke when a SendGrid email is received
		/// </param>
		/// <remarks>
		/// By design, SendGrid emails are not considered as webhooks, since they implement
		/// other methodologies and designs, but instead they are non-signed HTTP requests: 
		/// this method provides a utility to exploit the infrastructure
		/// provided by the framework to listen and handle incoming SendGrid emails, as if
		/// they actually were webhooks.
		/// </remarks>
		/// <returns>
		/// Returns the instance of the <see cref="IApplicationBuilder"/> to extend
		/// </returns>
		public static IApplicationBuilder MapSendGridEmail(this IApplicationBuilder app, string path, Func<SendGridEmail, Task> handler)
			=> app.MapWebhook<SendGridEmail>(path, handler);

		/// <summary>
		/// Maps a SendGrid email handler to the given <paramref name="path"/>
		/// </summary>
		/// <typeparam name="T1">
		/// The type of the first argument of the handler
		/// </typeparam>
		/// <param name="app">
		/// The instance of the <see cref="IApplicationBuilder"/> to extend
		/// </param>
		/// <param name="path">
		/// The path where to listen for incoming SendGrid emails
		/// </param>
		/// <param name="handler">
		/// The asynchronous handler to invoke when a SendGrid email is received
		/// </param>
		/// <remarks>
		/// By design, SendGrid emails are not considered as webhooks, since they implement
		/// other methodologies and designs, but instead they are non-signed HTTP requests: 
		/// this method provides a utility to exploit the infrastructure
		/// provided by the framework to listen and handle incoming SendGrid emails, as if
		/// they actually were webhooks.
		/// </remarks>
		/// <returns>
		/// Returns the instance of the <see cref="IApplicationBuilder"/> to extend
		/// </returns>
		public static IApplicationBuilder MapSendGridEmail<T1>(this IApplicationBuilder app, string path, Func<SendGridEmail, T1, Task> handler)
			=> app.MapWebhook<SendGridEmail, T1>(path, handler);

		/// <summary>
		/// Maps a SendGrid email handler to the given <paramref name="path"/>
		/// </summary>
		/// <typeparam name="T1">
		/// The type of the first argument of the handler
		/// </typeparam>
		/// <typeparam name="T2">
		/// The type of the second argument of the handler
		/// </typeparam>
		/// <param name="app">
		/// The instance of the <see cref="IApplicationBuilder"/> to extend
		/// </param>
		/// <param name="path">
		/// The path where to listen for incoming SendGrid emails
		/// </param>
		/// <param name="handler">
		/// The asynchronous handler to invoke when a SendGrid email is received
		/// </param>
		/// <remarks>
		/// By design, SendGrid emails are not considered as webhooks, since they implement
		/// other methodologies and designs, but instead they are non-signed HTTP requests: 
		/// this method provides a utility to exploit the infrastructure
		/// provided by the framework to listen and handle incoming SendGrid emails, as if
		/// they actually were webhooks.
		/// </remarks>
		/// <returns>
		/// Returns the instance of the <see cref="IApplicationBuilder"/> to extend
		/// </returns>
		public static IApplicationBuilder MapSendGridEmail<T1, T2>(this IApplicationBuilder app, string path, Func<SendGridEmail, T1, T2, Task> handler)
			=> app.MapWebhook<SendGridEmail, T1, T2>(path, handler);

		/// <summary>
		/// Maps a SendGrid email handler to the given <paramref name="path"/>
		/// </summary>
		/// <typeparam name="T1">
		/// The type of the first argument of the handler
		/// </typeparam>
		/// <typeparam name="T2">
		/// The type of the second argument of the handler
		/// </typeparam>
		/// <typeparam name="T3">
		/// The type of the third argument of the handler
		/// </typeparam>
		/// <param name="app">
		/// The instance of the <see cref="IApplicationBuilder"/> to extend
		/// </param>
		/// <param name="path">
		/// The path where to listen for incoming SendGrid emails
		/// </param>
		/// <param name="handler">
		/// The asynchronous handler to invoke when a SendGrid email is received
		/// </param>
		/// <remarks>
		/// By design, SendGrid emails are not considered as webhooks, since they implement
		/// other methodologies and designs, but instead they are non-signed HTTP requests: 
		/// this method provides a utility to exploit the infrastructure
		/// provided by the framework to listen and handle incoming SendGrid emails, as if
		/// they actually were webhooks.
		/// </remarks>
		/// <returns>
		/// Returns the instance of the <see cref="IApplicationBuilder"/> to extend
		/// </returns>
		public static IApplicationBuilder MapSendGridEmail<T1, T2, T3>(this IApplicationBuilder app, string path, Func<SendGridEmail, T1, T2, T3, Task> handler)
			=> app.MapWebhook<SendGridEmail, T1, T2, T3>(path, handler);
	}
}
