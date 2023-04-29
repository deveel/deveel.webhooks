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
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Deveel.Webhooks.SendGrid;

using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

namespace Deveel.Webhooks {
	public static class ServiceCollectionExtensions {
		public static WebhookReceiverBuilder<SendGridWebhook> AddSendGridReceiver(this IServiceCollection services) {
			var builder = services.AddWebhookReceiver<SendGridWebhook>()
				.Configure(_ => { });

			services.AddTransient<IPostConfigureOptions<WebhookReceiverOptions<SendGridWebhook>>, ConfigureWebhookReceiverOptions>();
			services.AddTransient<IPostConfigureOptions<WebhookVerificationOptions<SendGridWebhook>>, ConfigureWebhookVerificationOptions>();

			return builder;
		}

		public static WebhookReceiverBuilder<SendGridWebhook> AddSendGridReceiver(this IServiceCollection services, Action<SendGridReceiverOptions> configure) {
			services.Configure(configure);

			return services.AddSendGridReceiver();
		}

		public static WebhookReceiverBuilder<SendGridWebhook> AddSendGridReceiver(this IServiceCollection services, SendGridReceiverOptions options) {
			services.AddSingleton(Options.Create(options));

			return services.AddSendGridReceiver();
		}

		public static WebhookReceiverBuilder<SendGridEmail> AddSendGridEmailReceiver(this IServiceCollection services) {
			var builder = services.AddWebhookReceiver<SendGridEmail>()
				.Configure(_ => { });

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
