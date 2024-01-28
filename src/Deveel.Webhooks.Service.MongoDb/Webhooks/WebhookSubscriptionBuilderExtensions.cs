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
	/// Provides extensions to the <see cref="WebhookSubscriptionBuilder{TSubscription}"/>
	/// to register the MongoDB storage.
	/// </summary>
    public static class WebhookSubscriptionBuilderExtensions {
        /// <summary>
        /// Registers the MongoDB storage for the webhook subscriptions.
        /// </summary>
        /// <typeparam name="TSubscription">
        /// The type of the subscription handled by the storage,
        /// that must be derived from <see cref="MongoWebhookSubscription"/>.
        /// </typeparam>
        /// <param name="builder">
        /// The webhook subscription service builder used to register the storage.
        /// </param>
        /// <returns>
        /// Returns an instance of <see cref="MongoDbWebhookStorageBuilder{TSubscription}"/>
        /// used to further configure the storage.
        /// </returns>
        public static MongoDbWebhookStorageBuilder<TSubscription> UseMongoDb<TSubscription>(this WebhookSubscriptionBuilder<TSubscription> builder)
            where TSubscription : MongoWebhookSubscription
            => new MongoDbWebhookStorageBuilder<TSubscription>(builder);

        /// <summary>
        /// Registers the MongoDB storage for the webhook subscriptions,
        /// using the given connection string to connect to the database.
        /// </summary>
        /// <typeparam name="TSubscription">
        /// The type of the subscription handled by the storage,
        /// that must be derived from <see cref="MongoWebhookSubscription"/>.
        /// </typeparam>
        /// <param name="builder">
        /// The webhook subscription service builder used to register the storage.
        /// </param>
        /// <param name="connectionString">
        /// The connection string to be used to connect to the MongoDB database.
        /// </param>
        /// <returns>
        /// Returns an instance of <see cref="MongoDbWebhookStorageBuilder{TSubscription}"/>
        /// used to further configure the storage.
        /// </returns>
        public static MongoDbWebhookStorageBuilder<TSubscription> UseMongoDb<TSubscription>(this WebhookSubscriptionBuilder<TSubscription> builder, string connectionString)
			where TSubscription : MongoWebhookSubscription
			=> builder.UseMongoDb().WithConnectionString(connectionString);

        /// <summary>
        /// Registers the MongoDB storage for the webhook subscriptions,
        /// that can be further configured using the given <paramref name="configure"/>
        /// function provided.
        /// </summary>
        /// <typeparam name="TSubscription">
        /// The type of the subscription handled by the storage,
        /// that must be derived from <see cref="MongoWebhookSubscription"/>.
        /// </typeparam>
        /// <param name="builder">
        /// The webhook subscription service builder used to register the storage.
        /// </param>
        /// <param name="configure">
        /// The function used to configure the storage.
        /// </param>
        /// <returns>
        /// Returns the instance of <see cref="WebhookSubscriptionBuilder{TSubscription}"/>
        /// with the registered storage.
        /// </returns>
		public static WebhookSubscriptionBuilder<TSubscription> UseMongoDb<TSubscription>(this WebhookSubscriptionBuilder<TSubscription> builder, Action<MongoDbWebhookStorageBuilder<TSubscription>> configure)
			where TSubscription : MongoWebhookSubscription {
			var storageBuilder = new MongoDbWebhookStorageBuilder<TSubscription>(builder);
			configure?.Invoke(storageBuilder);

			return builder;
		}
	}
}
