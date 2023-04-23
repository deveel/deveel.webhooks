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

using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace Deveel.Webhooks {
	public sealed class WebhookSubscriptionBuilder<TSubscription> where TSubscription : class, IWebhookSubscription {
		public WebhookSubscriptionBuilder(IServiceCollection services) {
			Services = services ?? throw new ArgumentNullException(nameof(services));

			RegisterDefaults();
		}

		public IServiceCollection Services { get; }

		private void RegisterDefaults() {
			Services.TryAddScoped<WebhookSubscriptionManager<TSubscription>>();

			Services.TryAddSingleton<IWebhookSubscriptionValidator<TSubscription>, WebhookSubscriptionValidator<TSubscription>>();

			Services.TryAddScoped<IWebhookSubscriptionResolver, DefaultWebhookSubscriptionResolver<TSubscription>>();
		}

		public WebhookSubscriptionBuilder<TSubscription> UseNotifier<TWebhook>(Action<WebhookNotifierBuilder<TWebhook>> configure)
			where TWebhook : class, IWebhook {
			Services.AddWebhookNotifier<TWebhook>(configure);

			return this;
		}

		public WebhookSubscriptionBuilder<TSubscription> UseManager<TManager>(ServiceLifetime lifetime = ServiceLifetime.Scoped)
			where TManager : WebhookSubscriptionManager<TSubscription> {

			Services.TryAdd(new ServiceDescriptor(typeof(WebhookSubscriptionManager<TSubscription>), typeof(TManager), lifetime));

			if (typeof(TManager) != typeof(WebhookSubscriptionManager<TSubscription>))
				Services.Add(new ServiceDescriptor(typeof(TManager), typeof(TManager), lifetime));

			return this;
		}

		public WebhookSubscriptionBuilder<TSubscription> UseManager()
			=> UseManager<WebhookSubscriptionManager<TSubscription>>();


		public WebhookSubscriptionBuilder<TSubscription> AddValidator<TValidator>(ServiceLifetime lifetime = ServiceLifetime.Singleton)
			where TValidator : class, IWebhookSubscriptionValidator<TSubscription> {
			Services.Add(new ServiceDescriptor(typeof(IWebhookSubscriptionValidator<TSubscription>), typeof(TValidator), lifetime));
			Services.Add(new ServiceDescriptor(typeof(TValidator), typeof(TValidator), lifetime));

			return this;
		}
	}
}
