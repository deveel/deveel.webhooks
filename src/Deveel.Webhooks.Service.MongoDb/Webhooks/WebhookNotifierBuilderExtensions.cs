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

namespace Deveel.Webhooks {
    /// <summary>
    /// Provides extensions to the <see cref="WebhookNotifierBuilder{TWebhook}"/>
    /// to register the MongoDB storage for the notifier.
    /// </summary>
    public static class WebhookNotifierBuilderExtensions {
		/// <summary>
		/// Registers the MongoDB storage for resolving
		/// webhook subscriptions to the notifier.
		/// </summary>
		/// <typeparam name="TWebhook">
		/// The type of the webhook to be notified.
		/// </typeparam>
		/// <param name="builder">
		/// The builder of the notifier service where to register
		/// the resolver.
		/// </param>
		/// <returns>
		/// Returns the builder to continue the configuration.
		/// </returns>
		public static WebhookNotifierBuilder<TWebhook> UseMongoSubscriptionResolver<TWebhook>(this WebhookNotifierBuilder<TWebhook> builder)
			where TWebhook : class {
			return builder
				.UseDefaultSubscriptionResolver(typeof(MongoWebhookSubscription));
		}
	}
}
