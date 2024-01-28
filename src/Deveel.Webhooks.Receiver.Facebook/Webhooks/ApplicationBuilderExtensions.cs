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

using System.Diagnostics.CodeAnalysis;

using Deveel.Webhooks.Facebook;

using Microsoft.AspNetCore.Builder;

namespace Deveel.Webhooks {
	/// <summary>
	/// Extends the <see cref="IApplicationBuilder"/> to provide methods
	/// to map a Facebook Webhook endpoint.
	/// </summary>
	[ExcludeFromCodeCoverage]
    public static class ApplicationBuilderExtensions {
		/// <summary>
		/// Maps a Facebook Webhook endpoint to the specified <paramref name="path"/>.
		/// </summary>
		/// <param name="app">
		/// The <see cref="IApplicationBuilder"/> to use to map the endpoint.
		/// </param>
		/// <param name="path">
		/// The relative path where the Facebook Webhook is delivered.
		/// </param>
		/// <param name="options">
		/// A set of options to configure the handling of the Webhook by
		/// the middleware.
		/// </param>
		/// <remarks>
		/// The underlying middleware will attempt to receive a Facebook Webhook
		/// at the relative path within the application, and then invoke all
		/// the registered handlers to process the incoming webhook.
		/// </remarks>
		/// <returns>
		/// Returns the <see cref="IApplicationBuilder"/> to continue the
		/// building of the application.
		/// </returns>
		public static IApplicationBuilder MapFacebookWebhook(this IApplicationBuilder app, string path = "/facebook", FacebookWebhookHandlingOptions? options = null) {
			app.MapWebhook<FacebookWebhook>(path, new WebhookHandlingOptions {
				ResponseStatusCode = 200,
				InvalidStatusCode = 400,
				MaxParallelThreads = options?.MaxParallelThreads ?? 1,
				ExecutionMode = options?.ExecutionMode
			});
			return app;
		}

		/// <summary>
		/// Maps a Facebook Webhook endpoint to the specified <paramref name="path"/>,
		/// using the provided delegate to handle the incoming webhooks.
		/// </summary>
		/// <param name="app">
		/// The <see cref="IApplicationBuilder"/> to use to map the endpoint.
		/// </param>
		/// <param name="path">
		/// The relative path where the Facebook Webhook is delivered.
		/// </param>
		/// <param name="handler">
		/// The asynchronous delegate that is invoked to handle the incoming webhook.
		/// </param>
		/// <remarks>
		/// The underlying middleware will attempt to receive a Facebook Webhook
		/// at the relative path within the application, and then invoke the
		/// provided <paramref name="handler"/> to process the incoming webhook.
		/// </remarks>
		/// <returns>
		/// Returns the <see cref="IApplicationBuilder"/> to continue the
		/// building of the application.
		/// </returns>
		public static IApplicationBuilder MapFacebookWebhook(this IApplicationBuilder app, string path, Func<FacebookWebhook, Task> handler)
			=> app.MapWebhook(path, handler);

        /// <summary>
        /// Maps a Facebook Webhook endpoint to the specified <paramref name="path"/>,
        /// using the provided delegate to handle the incoming webhooks.
        /// </summary>
		/// <typeparam name="T1">The type of the first parameter, after the webhook, of the handler</typeparam>
        /// <param name="app">
        /// The <see cref="IApplicationBuilder"/> to use to map the endpoint.
        /// </param>
        /// <param name="path">
        /// The relative path where the Facebook Webhook is delivered.
        /// </param>
        /// <param name="handler">
        /// The asynchronous delegate that is invoked to handle the incoming webhook.
        /// </param>
        /// <remarks>
        /// The underlying middleware will attempt to receive a Facebook Webhook
        /// at the relative path within the application, and then invoke the
        /// provided <paramref name="handler"/> to process the incoming webhook.
        /// </remarks>
        /// <returns>
        /// Returns the <see cref="IApplicationBuilder"/> to continue the
        /// building of the application.
        /// </returns>
        public static IApplicationBuilder MapFacebookWebhook<T1>(this IApplicationBuilder app, string path, Func<FacebookWebhook, T1, Task> handler)
			=> app.MapWebhook(path, handler);

        /// <summary>
        /// Maps a Facebook Webhook endpoint to the specified <paramref name="path"/>,
        /// using the provided delegate to handle the incoming webhooks.
        /// </summary>
        /// <typeparam name="T1">The type of the first parameter, after the webhook, of the handler</typeparam>
		/// <typeparam name="T2">The type of the second parameter, after the webhook, of the handler</typeparam>
        /// <param name="app">
        /// The <see cref="IApplicationBuilder"/> to use to map the endpoint.
        /// </param>
        /// <param name="path">
        /// The relative path where the Facebook Webhook is delivered.
        /// </param>
        /// <param name="handler">
        /// The asynchronous delegate that is invoked to handle the incoming webhook.
        /// </param>
        /// <remarks>
        /// The underlying middleware will attempt to receive a Facebook Webhook
        /// at the relative path within the application, and then invoke the
        /// provided <paramref name="handler"/> to process the incoming webhook.
        /// </remarks>
        /// <returns>
        /// Returns the <see cref="IApplicationBuilder"/> to continue the
        /// building of the application.
        /// </returns>
        public static IApplicationBuilder MapFacebookWebhook<T1, T2>(this IApplicationBuilder app, string path, Func<FacebookWebhook, T1, T2, Task> handler)
			=> app.MapWebhook(path, handler);

        /// <summary>
        /// Maps a Facebook Webhook endpoint to the specified <paramref name="path"/>,
        /// using the provided delegate to handle the incoming webhooks.
        /// </summary>
        /// <typeparam name="T1">The type of the first parameter, after the webhook, of the handler</typeparam>
        /// <typeparam name="T2">The type of the second parameter, after the webhook, of the handler</typeparam>
		/// <typeparam name="T3">The type of the third parameter, after the webhook, of the handler</typeparam>
        /// <param name="app">
        /// The <see cref="IApplicationBuilder"/> to use to map the endpoint.
        /// </param>
        /// <param name="path">
        /// The relative path where the Facebook Webhook is delivered.
        /// </param>
        /// <param name="handler">
        /// The asynchronous delegate that is invoked to handle the incoming webhook.
        /// </param>
        /// <remarks>
        /// The underlying middleware will attempt to receive a Facebook Webhook
        /// at the relative path within the application, and then invoke the
        /// provided <paramref name="handler"/> to process the incoming webhook.
        /// </remarks>
        /// <returns>
        /// Returns the <see cref="IApplicationBuilder"/> to continue the
        /// building of the application.
        /// </returns>
        public static IApplicationBuilder MapFacebookWebhook<T1, T2, T3>(this IApplicationBuilder app, string path, Func<FacebookWebhook, T1, T2, T3, Task> handler)
			=> app.MapWebhook(path, handler);

        /// <summary>
        /// Maps a Facebook Webhook endpoint to the specified <paramref name="path"/>,
        /// using the provided delegate to handle the incoming webhooks.
        /// </summary>
        /// <param name="app">
        /// The <see cref="IApplicationBuilder"/> to use to map the endpoint.
        /// </param>
        /// <param name="path">
        /// The relative path where the Facebook Webhook is delivered.
        /// </param>
        /// <param name="handler">
        /// The delegate that is invoked to handle the incoming webhook.
        /// </param>
        /// <remarks>
        /// The underlying middleware will attempt to receive a Facebook Webhook
        /// at the relative path within the application, and then invoke the
        /// provided <paramref name="handler"/> to process the incoming webhook.
        /// </remarks>
        /// <returns>
        /// Returns the <see cref="IApplicationBuilder"/> to continue the
        /// building of the application.
        /// </returns>
        public static IApplicationBuilder MapFacebookWebhook(this IApplicationBuilder app, string path, Action<FacebookWebhook> handler)
			=> app.MapWebhook(path, handler);

        /// <summary>
        /// Maps a Facebook Webhook endpoint to the specified <paramref name="path"/>,
        /// using the provided delegate to handle the incoming webhooks.
        /// </summary>
        /// <typeparam name="T1">The type of the first parameter, after the webhook, of the handler</typeparam>
        /// <param name="app">
        /// The <see cref="IApplicationBuilder"/> to use to map the endpoint.
        /// </param>
        /// <param name="path">
        /// The relative path where the Facebook Webhook is delivered.
        /// </param>
        /// <param name="handler">
        /// The delegate that is invoked to handle the incoming webhook.
        /// </param>
        /// <remarks>
        /// The underlying middleware will attempt to receive a Facebook Webhook
        /// at the relative path within the application, and then invoke the
        /// provided <paramref name="handler"/> to process the incoming webhook.
        /// </remarks>
        /// <returns>
        /// Returns the <see cref="IApplicationBuilder"/> to continue the
        /// building of the application.
        /// </returns>
        public static IApplicationBuilder MapFacebookWebhook<T1>(this IApplicationBuilder app, string path, Action<FacebookWebhook, T1> handler)
			=> app.MapWebhook(path, handler);

        /// <summary>
        /// Maps a Facebook Webhook endpoint to the specified <paramref name="path"/>,
        /// using the provided delegate to handle the incoming webhooks.
        /// </summary>
        /// <typeparam name="T1">The type of the first parameter, after the webhook, of the handler</typeparam>
        /// <typeparam name="T2">The type of the second parameter, after the webhook, of the handler</typeparam>
        /// <param name="app">
        /// The <see cref="IApplicationBuilder"/> to use to map the endpoint.
        /// </param>
        /// <param name="path">
        /// The relative path where the Facebook Webhook is delivered.
        /// </param>
        /// <param name="handler">
        /// The delegate that is invoked to handle the incoming webhook.
        /// </param>
        /// <remarks>
        /// The underlying middleware will attempt to receive a Facebook Webhook
        /// at the relative path within the application, and then invoke the
        /// provided <paramref name="handler"/> to process the incoming webhook.
        /// </remarks>
        /// <returns>
        /// Returns the <see cref="IApplicationBuilder"/> to continue the
        /// building of the application.
        /// </returns>
        public static IApplicationBuilder MapFacebookWebhook<T1, T2>(this IApplicationBuilder app, string path, Action<FacebookWebhook, T1, T2> handler)
			=> app.MapWebhook(path, handler);

        /// <summary>
        /// Maps a Facebook Webhook endpoint to the specified <paramref name="path"/>,
        /// using the provided delegate to handle the incoming webhooks.
        /// </summary>
        /// <typeparam name="T1">The type of the first parameter, after the webhook, of the handler</typeparam>
        /// <typeparam name="T2">The type of the second parameter, after the webhook, of the handler</typeparam>
        /// <typeparam name="T3">The type of the third parameter, after the webhook, of the handler</typeparam>
        /// <param name="app">
        /// The <see cref="IApplicationBuilder"/> to use to map the endpoint.
        /// </param>
        /// <param name="path">
        /// The relative path where the Facebook Webhook is delivered.
        /// </param>
        /// <param name="handler">
        /// The delegate that is invoked to handle the incoming webhook.
        /// </param>
        /// <remarks>
        /// The underlying middleware will attempt to receive a Facebook Webhook
        /// at the relative path within the application, and then invoke the
        /// provided <paramref name="handler"/> to process the incoming webhook.
        /// </remarks>
        /// <returns>
        /// Returns the <see cref="IApplicationBuilder"/> to continue the
        /// building of the application.
        /// </returns>
        public static IApplicationBuilder MapFacebookWebhook<T1, T2, T3>(this IApplicationBuilder app, string path, Action<FacebookWebhook, T1, T2, T3> handler)
			=> app.MapWebhook(path, handler);

        /// <summary>
        /// Maps the verification endpoint that Facebook uses to verify
        /// the receiver of the webhooks.
        /// </summary>
        /// <param name="app">
        /// The <see cref="IApplicationBuilder"/> to use to map the endpoint.
        /// </param>
        /// <param name="path">
        /// The relative path where the Facebook platform will issue a
        /// request of verification
        /// </param>
        /// <returns>
        /// Returns the <see cref="IApplicationBuilder"/> to continue the
        /// building of the application.
        /// </returns>
		public static IApplicationBuilder MapFacebookVerify(this IApplicationBuilder app, string path = "/facebook/verify")
			=> app.MapWebhookVerify<FacebookWebhook>(path);
	}
}
