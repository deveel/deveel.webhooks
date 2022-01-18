// Copyright 2022 Deveel
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

using Deveel.Webhooks;

using Microsoft.Extensions.DependencyInjection;

namespace Deveel.Webhooks {
	public static class WebhookReceiverConfigurationBuilderExtensions {
		public static IWebhookReceiverConfigurationBuilder Configure(this IWebhookReceiverConfigurationBuilder builder, Action<WebhookReceiveOptions> configure) {
			if (configure != null)
				builder.ConfigureServices(services => services.Configure(configure));

			return builder;
		}

		public static IWebhookReceiverConfigurationBuilder AddWebhookOptions(this IWebhookReceiverConfigurationBuilder builder, WebhookReceiveOptions options) {
			return builder.ConfigureServices(services => services.AddSingleton<WebhookReceiveOptions>(options));
		}

		public static IWebhookReceiverConfigurationBuilder AddHttpReceiver<TReceiver, TWebhook>(this IWebhookReceiverConfigurationBuilder builder, ServiceLifetime lifetime = ServiceLifetime.Scoped)
			where TReceiver : class, IHttpWebhookReceiver<TWebhook>
			where TWebhook : class {
			builder.ConfigureServices(services => {
				services.Add(new ServiceDescriptor(typeof(IHttpWebhookReceiver<TWebhook>), typeof(TReceiver), lifetime));
				services.Add(new ServiceDescriptor(typeof(TReceiver), lifetime));
			});

			return builder;
		}

		public static IWebhookReceiverConfigurationBuilder AddHttpReceiver<TReceiver, TWebhook>(this IWebhookReceiverConfigurationBuilder builder, TReceiver receiver)
			where TReceiver : class, IHttpWebhookReceiver<TWebhook>
			where TWebhook : class {
			builder.ConfigureServices(services => services
				.AddSingleton<IHttpWebhookReceiver<TWebhook>>(receiver)
				.AddSingleton(receiver));
			return builder;
		}

		public static IWebhookReceiverConfigurationBuilder AddHttpReceiver<T>(this IWebhookReceiverConfigurationBuilder builder)
			where T : class
			=> builder.AddHttpReceiver<DefaultHttptWebhookReceiver<T>, T>();

	}
}
