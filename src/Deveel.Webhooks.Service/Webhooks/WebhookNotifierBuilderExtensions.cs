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

using Microsoft.Extensions.DependencyInjection;

namespace Deveel.Webhooks {
	/// <summary>
	/// Extends the <see cref="WebhookNotifierBuilder{TWebhook}"/> class
	/// to register the default subscription resolver
	/// </summary>
	public static class WebhookNotifierBuilderExtensions {
		/// <summary>
		/// Registers the default subscription resolver for the given webhook type
		/// and that is based on the given subscription type.
		/// </summary>
		/// <typeparam name="TWebhook"></typeparam>
		/// <param name="builder"></param>
		/// <param name="subscriptionType"></param>
		/// <param name="lifetime"></param>
		/// <returns></returns>
		/// <exception cref="ArgumentException"></exception>
		public static WebhookNotifierBuilder<TWebhook> UseDefaultTenantSubscriptionResolver<TWebhook>(this WebhookNotifierBuilder<TWebhook> builder, Type subscriptionType, ServiceLifetime lifetime = ServiceLifetime.Scoped)
			where TWebhook : class {
			if (!typeof(IWebhookSubscription).IsAssignableFrom(subscriptionType))
				throw new ArgumentException("The type specified is not a subscription type", nameof(subscriptionType));

			var resolverType = typeof(TenantWebhookSubscriptionResolver<>).MakeGenericType(subscriptionType);
			return builder.UseTenantSubscriptionResolver(resolverType, lifetime);
		}

		/// <summary>
		/// Registers the default subscription resolver for the given webhook type
		/// and that is based on the given subscription type.
		/// </summary>
		/// <typeparam name="TWebhook"></typeparam>
		/// <param name="builder"></param>
		/// <param name="subscriptionType"></param>
		/// <param name="lifetime"></param>
		/// <returns></returns>
		/// <exception cref="ArgumentException"></exception>
		public static WebhookNotifierBuilder<TWebhook> UseDefaultSubscriptionResolver<TWebhook>(this WebhookNotifierBuilder<TWebhook> builder, Type subscriptionType, ServiceLifetime lifetime = ServiceLifetime.Scoped)
			where TWebhook : class {
			if (!typeof(IWebhookSubscription).IsAssignableFrom(subscriptionType))
				throw new ArgumentException("The type specified is not a subscription type", nameof(subscriptionType));

			var resolverType = typeof(WebhookSubscriptionResolver<>).MakeGenericType(subscriptionType);
			return builder.UseSubscriptionResolver(resolverType, lifetime);
		}
	}
}
