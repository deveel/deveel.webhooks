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

using Microsoft.Extensions.DependencyInjection;

namespace Deveel.Webhooks {
	public static class WebhookReceiverConfigurationBuilderExtensions {
		public static IWebhookReceiverConfigurationBuilder AddReceiver<TReceiver, TWebhook>(this IWebhookReceiverConfigurationBuilder builder, ServiceLifetime lifetime = ServiceLifetime.Singleton)
			where TReceiver : class, IWebhookReceiver<TWebhook>
			where TWebhook : class {
			return builder.ConfigureServices(services => {
				services.Add(new ServiceDescriptor(typeof(IWebhookReceiver<TWebhook>), typeof(TReceiver), lifetime));
				services.Add(new ServiceDescriptor(typeof(TReceiver), typeof(TReceiver), lifetime));
			});
		}

		public static IWebhookReceiverConfigurationBuilder AddReceiver<TWebhook>(this IWebhookReceiverConfigurationBuilder builder, ServiceLifetime lifetime = ServiceLifetime.Singleton)
			where TWebhook : class
			=> builder.AddReceiver<DefaultWebhookReceiver<TWebhook>, TWebhook>(lifetime);
	}
}
