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

using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Deveel.Webhooks.Twilio;

using Microsoft.AspNetCore.Builder;

namespace Deveel.Webhooks {
	/// <summary>
	/// Extends the <see cref="IApplicationBuilder"/> to provide methods
	/// to map a Twilio webhook endpoint.
	/// </summary>
	[ExcludeFromCodeCoverage]
	public static class ApplicationBuilderExtensions {
		/// <summary>
		/// Maps a Twilio webhook endpoint to the specified <paramref name="path"/>.
		/// </summary>
		/// <param name="builder">
		/// The <see cref="IApplicationBuilder"/> to use to map the endpoint.
		/// </param>
		/// <param name="path">
		/// The relative path to map the endpoint to.
		/// </param>
		/// <returns>
		/// Returns the <see cref="IApplicationBuilder"/> to continue the configuration.
		/// </returns>
		public static IApplicationBuilder MapTwilioWebhook(this IApplicationBuilder builder, string path = "/webhook/twilio") {
			return builder.MapWebhook<TwilioWebhook>(path);
		}

		/// <summary>
		/// Maps a Twilio webhook endpoint to the specified <paramref name="path"/>
		/// </summary>
		/// <param name="builder">
		/// The <see cref="IApplicationBuilder"/> to use to map the endpoint.
		/// </param>
		/// <param name="path">
		/// The relative path to map the endpoint to.
		/// </param>
		/// <param name="handler">
		/// The delegate that is invoked when the webhook is received.
		/// </param>
		/// <returns>
		/// Returns the <see cref="IApplicationBuilder"/> to continue the configuration.
		/// </returns>
		public static IApplicationBuilder MapTwilioWebhook(this IApplicationBuilder builder, string path, Action<TwilioWebhook> handler)
			=> builder.MapWebhook(path, handler);

		/// <summary>
		/// Maps a Twilio webhook endpoint to the specified <paramref name="path"/>
		/// </summary>
		/// <typeparam name="T1">The type of the first argument, after the webhook, to be bound
		/// to the handler function</typeparam>
		/// <param name="builder">
		/// The <see cref="IApplicationBuilder"/> to use to map the endpoint.
		/// </param>
		/// <param name="path">
		/// The relative path to map the endpoint to.
		/// </param>
		/// <param name="handler">
		/// The delegate that is invoked when the webhook is received.
		/// </param>
		/// <returns>
		/// Returns the <see cref="IApplicationBuilder"/> to continue the configuration.
		/// </returns>
		public static IApplicationBuilder MapTwilioWebhook<T1>(this IApplicationBuilder builder, string path, Action<TwilioWebhook, T1> handler)
			=> builder.MapWebhook(path, handler);

		/// <summary>
		/// Maps a Twilio webhook endpoint to the specified <paramref name="path"/>
		/// </summary>
		/// <typeparam name="T1">The type of the first argument, after the webhook, to be bound
		/// to the handler function</typeparam>
		/// <typeparam name="T2">The type of the second argument, after the webhook, to be bound
		/// to the handler function</typeparam>
		/// <param name="builder">
		/// The <see cref="IApplicationBuilder"/> to use to map the endpoint.
		/// </param>
		/// <param name="path">
		/// The relative path to map the endpoint to.
		/// </param>
		/// <param name="handler">
		/// The delegate that is invoked when the webhook is received.
		/// </param>
		/// <returns>
		/// Returns the <see cref="IApplicationBuilder"/> to continue the configuration.
		/// </returns>
		public static IApplicationBuilder MapTwilioWebhook<T1, T2>(this IApplicationBuilder builder, string path, Action<TwilioWebhook, T1, T2> handler)
			=> builder.MapWebhook(path, handler);

		/// <summary>
		/// Maps a Twilio webhook endpoint to the specified <paramref name="path"/>
		/// </summary>
		/// <typeparam name="T1">The type of the first argument, after the webhook, to be bound
		/// to the handler function</typeparam>
		/// <typeparam name="T2">The type of the second argument, after the webhook, to be bound
		/// to the handler function</typeparam>
		/// <typeparam name="T3">The type of the third argument, after the webhook, to be bound
		/// to the handler function</typeparam>
		/// <param name="builder">
		/// The <see cref="IApplicationBuilder"/> to use to map the endpoint.
		/// </param>
		/// <param name="path">
		/// The relative path to map the endpoint to.
		/// </param>
		/// <param name="handler">
		/// The delegate that is invoked when the webhook is received.
		/// </param>
		/// <returns>
		/// Returns the <see cref="IApplicationBuilder"/> to continue the configuration.
		/// </returns>
		public static IApplicationBuilder MapTwilioWebhook<T1, T2, T3>(this IApplicationBuilder builder, string path, Action<TwilioWebhook, T1, T2, T3> handler)
			=> builder.MapWebhook(path, handler);

		/// <summary>
		/// Maps a Twilio webhook endpoint to the specified <paramref name="path"/>
		/// </summary>
		/// <param name="builder">
		/// The <see cref="IApplicationBuilder"/> to use to map the endpoint.
		/// </param>
		/// <param name="path">
		/// The relative path to map the endpoint to.
		/// </param>
		/// <param name="handler">
		/// The delegate that is invoked when the webhook is received.
		/// </param>
		/// <returns>
		/// Returns the <see cref="IApplicationBuilder"/> to continue the configuration.
		/// </returns>
		public static IApplicationBuilder MapTwilioWebhook(this IApplicationBuilder builder, string path, Func<TwilioWebhook, Task> handler)
			=> builder.MapWebhook(path, handler);

		/// <summary>
		/// Maps a Twilio webhook endpoint to the specified <paramref name="path"/>
		/// </summary>
		/// <typeparam name="T1">The type of the first argument, after the webhook, to be bound
		/// to the handler function</typeparam>
		/// <param name="builder">
		/// The <see cref="IApplicationBuilder"/> to use to map the endpoint.
		/// </param>
		/// <param name="path">
		/// The relative path to map the endpoint to.
		/// </param>
		/// <param name="handler">
		/// The delegate that is invoked when the webhook is received.
		/// </param>
		/// <returns>
		/// Returns the <see cref="IApplicationBuilder"/> to continue the configuration.
		/// </returns>
		public static IApplicationBuilder MapTwilioWebhook<T1>(this IApplicationBuilder builder, string path, Func<TwilioWebhook, T1, Task> handler)
			=> builder.MapWebhook(path, handler);

		/// <summary>
		/// Maps a Twilio webhook endpoint to the specified <paramref name="path"/>
		/// </summary>
		/// <typeparam name="T1">The type of the first argument, after the webhook, to be bound
		/// to the handler function</typeparam>
		/// <typeparam name="T2">The type of the second argument, after the webhook, to be bound
		/// to the handler function</typeparam>
		/// <param name="builder">
		/// The <see cref="IApplicationBuilder"/> to use to map the endpoint.
		/// </param>
		/// <param name="path">
		/// The relative path to map the endpoint to.
		/// </param>
		/// <param name="handler">
		/// The delegate that is invoked when the webhook is received.
		/// </param>
		/// <returns>
		/// Returns the <see cref="IApplicationBuilder"/> to continue the configuration.
		/// </returns>
		public static IApplicationBuilder MapTwilioWebhook<T1, T2>(this IApplicationBuilder builder, string path, Func<TwilioWebhook, T1, T2, Task> handler)
			=> builder.MapWebhook(path, handler);

		/// <summary>
		/// Maps a Twilio webhook endpoint to the specified <paramref name="path"/>
		/// </summary>
		/// <typeparam name="T1">The type of the first argument, after the webhook, to be bound
		/// to the handler function</typeparam>
		/// <typeparam name="T2">The type of the second argument, after the webhook, to be bound
		/// to the handler function</typeparam>
		/// <typeparam name="T3">The type of the third argument, after the webhook, to be bound
		/// to the handler function</typeparam>
		/// <param name="builder">
		/// The <see cref="IApplicationBuilder"/> to use to map the endpoint.
		/// </param>
		/// <param name="path">
		/// The relative path to map the endpoint to.
		/// </param>
		/// <param name="handler">
		/// The delegate that is invoked when the webhook is received.
		/// </param>
		/// <returns>
		/// Returns the <see cref="IApplicationBuilder"/> to continue the configuration.
		/// </returns>
		public static IApplicationBuilder MapTwilioWebhook<T1, T2, T3>(this IApplicationBuilder builder, string path, Func<TwilioWebhook, T1, T2, T3, Task> handler)
			=> builder.MapWebhook(path, handler);
	}
}
