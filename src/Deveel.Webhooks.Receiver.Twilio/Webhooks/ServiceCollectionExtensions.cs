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

using Deveel.Webhooks.Twilio;

using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

namespace Deveel.Webhooks {
	/// <summary>
	/// Extends the <see cref="IServiceCollection"/> to provide methods
	/// to register a Twilio webhook receiver.
	/// </summary>
	public static class ServiceCollectionExtensions {
		/// <summary>
		/// Registers a Twilio webhook receiver in the application.
		/// </summary>
		/// <param name="services">
		/// The <see cref="IServiceCollection"/> to register the receiver.
		/// </param>
		/// <returns>
		/// Returns the <see cref="WebhookReceiverBuilder{TWebhook}"/> to continue the configuration.
		/// </returns>
		public static WebhookReceiverBuilder<TwilioWebhook> AddTwilioReceiver(this IServiceCollection services) {
			var builder = services.AddWebhookReceiver<TwilioWebhook>(_ => { });

			services.AddTransient<IPostConfigureOptions<WebhookReceiverOptions<TwilioWebhook>>, ConfigureWebhookReceiverOptions>();

			return builder;
		}

		/// <summary>
		/// Registers a Twilio webhook receiver in the application,
		/// providing a set of configuration options.
		/// </summary>
		/// <param name="services">
		/// The <see cref="IServiceCollection"/> to register the receiver.
		/// </param>
		/// <param name="options">
		/// The configuration options to use to configure the receiver.
		/// </param>
		/// <returns></returns>
		public static WebhookReceiverBuilder<TwilioWebhook> AddTwilioReceiver(this IServiceCollection services, TwilioReceiverOptions options) {
			services.AddSingleton(Options.Create(options));

			return services.AddTwilioReceiver();
		}

		/// <summary>
		/// Registers a Twilio webhook receiver in the application,
		/// configuring the options.
		/// </summary>
		/// <param name="services">
		/// The <see cref="IServiceCollection"/> to register the receiver.
		/// </param>
		/// <param name="configure">
		/// The delegate to use to configure the options.
		/// </param>
		/// <returns>
		/// Returns the <see cref="WebhookReceiverBuilder{TWebhook}"/> to continue the configuration.
		/// </returns>
		public static WebhookReceiverBuilder<TwilioWebhook> AddTwilioReceiver(this IServiceCollection services, Action<TwilioReceiverOptions> configure) {
			services.Configure(configure);

			return services.AddTwilioReceiver();
		}

		/// <summary>
		/// Registers a Twilio webhook receiver in the application,
		/// configuring the options from the given configuration section.
		/// </summary>
		/// <param name="services">
		/// The <see cref="IServiceCollection"/> to register the receiver.
		/// </param>
		/// <param name="sectionPath">
		/// The path to the configuration section to use to configure the receiver.
		/// </param>
		/// <returns>
		/// Returns the <see cref="WebhookReceiverBuilder{TWebhook}"/> to continue the configuration.
		/// </returns>
		public static WebhookReceiverBuilder<TwilioWebhook> AddTwilioReceiver(this IServiceCollection services, string sectionPath) {
			services.AddOptions<TwilioWebhook>()
				.BindConfiguration(sectionPath);

			return services.AddTwilioReceiver();
		}
	}
}
