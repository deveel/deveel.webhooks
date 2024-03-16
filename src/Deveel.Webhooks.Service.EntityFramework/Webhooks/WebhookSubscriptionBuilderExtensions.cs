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

namespace Deveel.Webhooks {
	/// <summary>
	/// Provides extensions to the <see cref="WebhookSubscriptionBuilder{TSubscription,TKey}"/>
	/// to configure the storage system based on Entity Framework.
	/// </summary>
    public static class WebhookSubscriptionBuilderExtensions {
		/// <summary>
		/// Instructs the builder to use Entity Framework as the storage system
		/// </summary>
		/// <typeparam name="TSubscription">
		/// The type of the <see cref="DbWebhookSubscription"/> entity to use
		/// </typeparam>
		/// <param name="builder">
		/// The instance of the <see cref="WebhookSubscriptionBuilder{TSubscription,TKey}"/> to
		/// extend with the Entity Framework storage system.
		/// </param>
		/// <returns>
		/// Returns an instance of <see cref="EntityWebhookStorageBuilder{TSubscription}"/>
		/// that can be used to configure the storage system.
		/// </returns>
        public static EntityWebhookStorageBuilder<TSubscription> UseEntityFramework<TSubscription>(this WebhookSubscriptionBuilder<TSubscription,string> builder)
            where TSubscription : DbWebhookSubscription
			=> new EntityWebhookStorageBuilder<TSubscription>(builder);

		/// <summary>
		/// Instructs the builder to use Entity Framework as the storage system
		/// </summary>
		/// <typeparam name="TSubscription">
		/// The type of the <see cref="DbWebhookSubscription"/> entity to use
		/// </typeparam>
		/// <param name="builder">
		/// The instance of the <see cref="WebhookSubscriptionBuilder{TSubscription,TKey}"/> to
		/// extend with the Entity Framework storage system.
		/// </param>
		/// <param name="configure">
		/// An action that receives an instance of <see cref="EntityWebhookStorageBuilder{TSubscription}"/>
		/// as input to configure the storage system.
		/// </param>
		/// <returns>
		/// Returns the same instance of <see cref="WebhookSubscriptionBuilder{TSubscription,TKey}"/>
		/// as the input, to allow chaining of calls.
		/// </returns>
		public static WebhookSubscriptionBuilder<TSubscription, string> UseEntityFramework<TSubscription>(this WebhookSubscriptionBuilder<TSubscription, string> builder, Action<EntityWebhookStorageBuilder<TSubscription>> configure)
			where TSubscription : DbWebhookSubscription {
			var storageBuilder = builder.UseEntityFramework();
			configure(storageBuilder);
			return builder;
		}
	}
}
