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
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Deveel.Webhooks.SendGrid;

using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

namespace Deveel.Webhooks {
	/// <summary>
	/// Extends the <see cref="IServiceCollection"/> with methods to
	/// register receivers for SendGrid webhooks and emails.
	/// </summary>
	public static class ServiceCollectionExtensions {
		/// <summary>
		/// Registers a receiver for the SendGrid webhooks.
		/// </summary>
		/// <param name="services">
		/// The collection of services where to register the receiver
		/// </param>
		/// <returns>
		/// The instance of the <see cref="WebhookReceiverBuilder{TWebhook}"/> that
		/// can be used to configure the receiver.
		/// </returns>
		public static WebhookReceiverBuilder<SendGridWebhook> AddSendGridReceiver(this IServiceCollection services) {
			var builder = services.AddWebhookReceiver<SendGridWebhook>(_ => { });

			services.AddTransient<IPostConfigureOptions<WebhookReceiverOptions<SendGridWebhook>>, ConfigureWebhookReceiverOptions>();

			return builder;
		}

		/// <summary>
		/// Registers a receiver for the SendGrid webhooks with
		/// the given configuration.
		/// </summary>
		/// <param name="services">
		/// The collection of services where to register the receiver
		/// </param>
		/// <param name="configure">
		/// A delegate to a method that can configure the SendGrid
		/// receiver options.
		/// </param>
		/// <returns>
		/// Returns the instance of the <see cref="WebhookReceiverBuilder{TWebhook}"/> that
		/// can be used to configure the receiver.
		/// </returns>
		public static WebhookReceiverBuilder<SendGridWebhook> AddSendGridReceiver(this IServiceCollection services, Action<SendGridReceiverOptions> configure) {
			services.Configure(configure);

			return services.AddSendGridReceiver();
		}

		/// <summary>
		/// Registers a receiver for the SendGrid webhooks with
		/// the given configuration.
		/// </summary>
		/// <param name="services">
		/// The collection of services where to register the receiver
		/// </param>
		/// <param name="options">
		/// An instance of the <see cref="SendGridReceiverOptions"/> that is
		/// used to configure the receiver.
		/// </param>
		/// <returns>
		/// Returns the instance of the <see cref="WebhookReceiverBuilder{TWebhook}"/> that
		/// can be used to configure the receiver.
		/// </returns>
		public static WebhookReceiverBuilder<SendGridWebhook> AddSendGridReceiver(this IServiceCollection services, SendGridReceiverOptions options) {
			services.AddSingleton(Options.Create(options));

			return services.AddSendGridReceiver();
		}

		/// <summary>
		/// Registers a default receiver for the SendGrid emails.
		/// </summary>
		/// <param name="services">
		/// The collection of services where to register the receiver
		/// </param>
		/// <returns>
		/// Returns the instance of the <see cref="WebhookReceiverBuilder{TWebhook}"/> that
		/// can be used to configure the receiver.
		/// </returns>
		public static WebhookReceiverBuilder<SendGridEmail> AddSendGridEmailReceiver(this IServiceCollection services) {
			var builder = services.AddWebhookReceiver<SendGridEmail>(_ => { });

			services.PostConfigure<WebhookReceiverOptions<SendGridEmail>>(options => {
				options.ContentFormats = WebhookContentFormats.Json | WebhookContentFormats.Form;
				options.JsonParser = new SystemTextWebhookJsonParser<SendGridEmail>(SendGridWebhookParser.Options);
				options.FormParser = new SendGridEmailFormParser();
				options.RootType = WebhookRootType.Object;
				// TODO: configure the signature through DKIM/SPF?
				options.VerifySignature = false;
			});

			return builder;
		}
	}
}
