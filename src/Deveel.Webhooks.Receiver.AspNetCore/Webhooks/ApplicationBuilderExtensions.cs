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

using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;

namespace Deveel.Webhooks {
	/// <summary>
	/// Extends the <see cref="IApplicationBuilder"/> to provide methods
	/// for receiving webhooks within an ASP.NET Core application request pipeline.
	/// </summary>
    public static class ApplicationBuilderExtensions {
		/// <summary>
		/// Adds a middleware to the application pipeline that receives webhooks
		/// of that are posted to the given path.
		/// </summary>
		/// <typeparam name="TWebhook">The type of the webhook to be received</typeparam>
		/// <param name="app">The application builder instance</param>
		/// <param name="path">The relative path to listen for webhook posts</param>
		/// <param name="options">The options for the execution of the handlers</param>
		/// <remarks>
		/// <para>
		/// The middleware will listen only for POST requests to the given path using
		/// the configurations and services registered in the application.
		/// </para>
		/// <para>
		/// Before this middleware can be used, the webhook receiver must be registered
		/// during the application startup.
		/// </para>
		/// </remarks>
		/// <returns>
		/// Returns the instance of the <see cref="IApplicationBuilder"/> that handles
		/// webhooks posted to the given path.
		/// </returns>
		public static IApplicationBuilder UseWebhookReceiver<TWebhook>(this IApplicationBuilder app, string path)
			where TWebhook : class {
			return app.MapWhen(
				context => context.Request.Method == "POST" && context.Request.Path.Equals(path),
				builder => builder.UseMiddleware<WebhookReceiverMiddleware<TWebhook>>()
			);
		}

        /// <summary>
        /// Adds a middleware to the application pipeline that provides a verification
        /// mechanism for the webhook requests.
        /// </summary>
        /// <typeparam name="TWebhook">The type of the webhook</typeparam>
        /// <param name="app">The application builder instance</param>
        /// <param name="method">The HTTP method to listen for requests</param>
        /// <param name="path">The relative path to listen for verification requests</param>
		/// <remarks>
		/// <para>
		/// Some service providers require a verification of the webhook requests before
		/// posting the webhook to the receiver: this middleware provides a mechanism to
		/// handle such requests and respond.
		/// </para>
		/// <para>
		/// If the provider does not require a verification, this middleware can be ignored,
		/// and it will not affect the normal operation of the webhook receiver.
		/// </para>
		/// </remarks>
        /// <returns>
		/// Returns an instance of the <see cref="IApplicationBuilder"/> that handles
		/// the verification requests.
		/// </returns>
        public static IApplicationBuilder UseWebhookVerifier<TWebhook>(this IApplicationBuilder app, string method, string path)
			where TWebhook : class
			=> app.MapWhen(
				context => context.Request.Method == method && context.Request.Path.Equals(path),
				builder => builder.UseMiddleware<WebhookRequestVerfierMiddleware<TWebhook>>()
			);

        /// <summary>
        /// Adds a middleware to the application pipeline that provides a verification
        /// mechanism for the webhook requests.
        /// </summary>
        /// <typeparam name="TWebhook">The type of the webhook</typeparam>
        /// <param name="app">The application builder instance</param>
        /// <param name="path">The relative path to listen for verification requests</param>
        /// <remarks>
		/// <para>
		/// By default this middleware will listen for GET requests to the given path.
		/// </para>
        /// <para>
        /// Some service providers require a verification of the webhook requests before
        /// posting the webhook to the receiver: this middleware provides a mechanism to
        /// handle such requests and respond.
        /// </para>
        /// <para>
        /// If the provider does not require a verification, this middleware can be ignored,
        /// and it will not affect the normal operation of the webhook receiver.
        /// </para>
        /// </remarks>
        /// <returns>
        /// Returns an instance of the <see cref="IApplicationBuilder"/> that handles
        /// the verification requests.
        /// </returns>
        public static IApplicationBuilder UseWebhookVerfier<TWebhook>(this IApplicationBuilder app, string path)
			where TWebhook : class
			=> app.UseWebhookVerifier<TWebhook>("GET", path);

        /// <summary>
        /// Adds a middleware to the application pipeline that receives webhooks
        /// of that are posted to the given path.
        /// </summary>
        /// <typeparam name="TWebhook">The type of the webhook to be received</typeparam>
        /// <param name="app">The application builder instance</param>
        /// <param name="path">The path to listen for webhook posts</param>
        /// <param name="receiver">The delegated function that is invoked by the middleware
		/// to handle the received webhook</param>
		/// <remarks>
		/// <para>
		/// The middleware will listen only for POST requests to the given path using
		/// the configurations registered at the application startup.
		/// </para>
		/// <para>
		/// Any instance of the <see cref="IWebhookReceiver{TWebhook}"/> registered will
		/// be ignored when using this middleware, and only the provided function will be
		/// invoked by the middleware.
		/// </para>
		/// </remarks>
		/// <returns>
		/// Returns the instance of the <see cref="IApplicationBuilder"/> that handles
		/// webhooks posted to the given path.
		/// </returns>
        public static IApplicationBuilder UseWebhookReceiver<TWebhook>(this IApplicationBuilder app, string path, Func<HttpContext, TWebhook, CancellationToken, Task> receiver)
			where TWebhook : class {
			return app.MapWhen(
				context => context.Request.Method == "POST" && context.Request.Path.Equals(path),
				builder => builder.UseMiddleware<WebhookDelegatedReceiverMiddleware<TWebhook>>(receiver)
			);
		}

        /// <summary>
        /// Adds a middleware to the application pipeline that receives webhooks
        /// of that are posted to the given path.
        /// </summary>
        /// <typeparam name="TWebhook">The type of the webhook to be received</typeparam>
        /// <param name="app">The application builder instance</param>
        /// <param name="path">The path to listen for webhook posts</param>
        /// <param name="receiver">The delegated function that is invoked by the middleware
        /// to handle the received webhook</param>
        /// <remarks>
        /// <para>
        /// The middleware will listen only for POST requests to the given path using
        /// the configurations registered at the application startup.
        /// </para>
        /// <para>
        /// Any instance of the <see cref="IWebhookReceiver{TWebhook}"/> registered will
        /// be ignored when using this middleware, and only the provided function will be
        /// invoked by the middleware.
        /// </para>
        /// </remarks>
        /// <returns>
        /// Returns the instance of the <see cref="IApplicationBuilder"/> that handles
        /// webhooks posted to the given path.
        /// </returns>
        public static IApplicationBuilder UseWebhookReceiver<TWebhook>(this IApplicationBuilder app, string path, Action<HttpContext, TWebhook> receiver)
			where TWebhook : class {
			return app.MapWhen(
				context => context.Request.Method == "POST" && context.Request.Path.Equals(path),
				builder => builder.UseMiddleware<WebhookDelegatedReceiverMiddleware<TWebhook>>(receiver)
			);
		}

	}
}
