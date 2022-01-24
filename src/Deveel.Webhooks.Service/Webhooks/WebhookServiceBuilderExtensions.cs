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

using Microsoft.Extensions.DependencyInjection.Extensions;

namespace Deveel.Webhooks {
	public static class WebhookServiceBuilderExtensions {
		public static IWebhookServiceBuilder UseDefaultSubscriptionResolver(this IWebhookServiceBuilder builder)
			=> builder.UseSubscriptionResolver<DefaultWebhookSubscriptionResolver>();

		public static IWebhookServiceBuilder UseSubscriptionManager<TManager>(this IWebhookServiceBuilder builder)
			where TManager : class, IWebhookSubscriptionManager {
			builder.ConfigureServices(services => {
				services.TryAddScoped<IWebhookSubscriptionResolver, DefaultWebhookSubscriptionResolver>();
			});

			return builder.Use<IWebhookSubscriptionManager, TManager>();
		}

		public static IWebhookServiceBuilder UseSubscriptionManager(this IWebhookServiceBuilder builder)
			=> builder.UseSubscriptionManager<WebhookSubscriptionManager>();
	}
}
