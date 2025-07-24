﻿// Copyright 2022-2025 Antonello Provenzano
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
		public static IApplicationBuilder MapWebhook<TWebhook>(this IApplicationBuilder app, string path, WebhookHandlingOptions? options = null)
			where TWebhook : class {
			return app.MapWhen(
				context => context.Request.Method == "POST" && context.Request.Path.Equals(path),
				builder => builder.UseMiddleware<WebhookReceiverMiddleware<TWebhook>>(options ?? new WebhookHandlingOptions())
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
        public static IApplicationBuilder MapWebhookVerify<TWebhook>(this IApplicationBuilder app, string method, string path)
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
        public static IApplicationBuilder MapWebhookVerify<TWebhook>(this IApplicationBuilder app, string path)
			where TWebhook : class
			=> app.MapWebhookVerify<TWebhook>("GET", path);

		private static IApplicationBuilder MapWebhookWithDelegate<TWebhook>(this IApplicationBuilder app, string path, Delegate handler)
			where TWebhook : class {
			return app.MapWhen(
				context => context.Request.Method == "POST" && context.Request.Path.Equals(path),
				builder => builder.UseMiddleware<WebhookDelegatedReceiverMiddleware<TWebhook>>(new WebhookHandlingOptions(), new DelegatedWebhookHandler<TWebhook>(handler))
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
		public static IApplicationBuilder MapWebhook<TWebhook>(this IApplicationBuilder app, string path, Func<TWebhook, Task> receiver)
			where TWebhook : class
			=> app.MapWebhookWithDelegate<TWebhook>(path, receiver);

		/// <summary>
		/// Adds a middleware to the application pipeline that receives webhooks
		/// of that are posted to the given path, injecting the provided arguments
		/// to the function.
		/// </summary>
		/// <typeparam name="TWebhook">The type of the webhook to be received</typeparam>
		/// <typeparam name="T1">The type of the first parameter of the delegated function</typeparam>
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
		public static IApplicationBuilder MapWebhook<TWebhook, T1>(this IApplicationBuilder app, string path, Func<TWebhook, T1, Task> receiver)
			where TWebhook : class
			=> app.MapWebhookWithDelegate<TWebhook>(path, receiver);

		/// <summary>
		/// Adds a middleware to the application pipeline that receives webhooks
		/// of that are posted to the given path, injecting the provided arguments
		/// to the function.
		/// </summary>
		/// <typeparam name="TWebhook">The type of the webhook to be received</typeparam>
		/// <typeparam name="T1">The type of the first parameter of the delegated function</typeparam>
		/// <typeparam name="T2">The type of the second parameter of the delegated function</typeparam>
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
		public static IApplicationBuilder MapWebhook<TWebhook, T1, T2>(this IApplicationBuilder app, string path, Func<TWebhook, T1, T2, Task> receiver)
			where TWebhook : class
			=> app.MapWebhookWithDelegate<TWebhook>(path, receiver);

		/// <summary>
		/// Adds a middleware to the application pipeline that receives webhooks
		/// of that are posted to the given path, injecting the provided arguments
		/// to the function.
		/// </summary>
		/// <typeparam name="TWebhook">The type of the webhook to be received</typeparam>
		/// <typeparam name="T1">The type of the first parameter of the delegated function</typeparam>
		/// <typeparam name="T2">The type of the second parameter of the delegated function</typeparam>
		/// <typeparam name="T3">The type of the third parameter of the delegated function</typeparam>
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
		public static IApplicationBuilder MapWebhook<TWebhook, T1, T2, T3>(this IApplicationBuilder app, string path, Func<TWebhook, T1, T2, T3, Task> receiver)
			where TWebhook : class
			=> app.MapWebhookWithDelegate<TWebhook>(path, receiver);

		/// <summary>
		/// Adds a middleware to the application pipeline that receives webhooks
		/// of that are posted to the given path
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
		public static IApplicationBuilder MapWebhook<TWebhook>(this IApplicationBuilder app, string path, Action<TWebhook> receiver)
			where TWebhook : class
			=> app.MapWebhookWithDelegate<TWebhook>(path, receiver);

		/// <summary>
		/// Adds a middleware to the application pipeline that receives webhooks
		/// of that are posted to the given path, injecting the provided arguments
		/// to the function.
		/// </summary>
		/// <typeparam name="TWebhook">The type of the webhook to be received</typeparam>
		/// <typeparam name="T1">The type of the first parameter of the delegated function</typeparam>
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
		public static IApplicationBuilder MapWebhook<TWebhook, T1>(this IApplicationBuilder app, string path, Action<TWebhook, T1> receiver)
			where TWebhook : class
			=> app.MapWebhookWithDelegate<TWebhook>(path, receiver);

		/// <summary>
		/// Adds a middleware to the application pipeline that receives webhooks
		/// of that are posted to the given path, injecting the provided arguments
		/// to the function.
		/// </summary>
		/// <typeparam name="TWebhook">The type of the webhook to be received</typeparam>
		/// <typeparam name="T1">The type of the first parameter of the delegated function</typeparam>
		/// <typeparam name="T2">The type of the second parameter of the delegated function</typeparam>
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
		public static IApplicationBuilder MapWebhook<TWebhook, T1, T2>(this IApplicationBuilder app, string path, Action<TWebhook, T1, T2> receiver)
			where TWebhook : class
			=> app.MapWebhookWithDelegate<TWebhook>(path, receiver);

		/// <summary>
		/// Adds a middleware to the application pipeline that receives webhooks
		/// of that are posted to the given path, injecting the provided arguments
		/// to the function.
		/// </summary>
		/// <typeparam name="TWebhook">The type of the webhook to be received</typeparam>
		/// <typeparam name="T1">The type of the first parameter of the delegated function</typeparam>
		/// <typeparam name="T2">The type of the second parameter of the delegated function</typeparam>
		/// <typeparam name="T3">The type of the third parameter of the delegated function</typeparam>
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
		public static IApplicationBuilder MapWebhook<TWebhook, T1, T2, T3>(this IApplicationBuilder app, string path, Action<TWebhook, T1, T2, T3> receiver)
			where TWebhook : class
			=> app.MapWebhookWithDelegate<TWebhook>(path, receiver);
	}
}
