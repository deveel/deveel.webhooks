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

using Finbuckle.MultiTenant;

using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace Deveel.Webhooks {
    /// <summary>
    /// A builder for configuring the storage of webhook subscriptions
    /// </summary>
    /// <typeparam name="TSubscription">
	/// The type of the <see cref="DbWebhookSubscription"/> entity to use
	/// </typeparam>
    public sealed class EntityWebhookStorageBuilder<TSubscription> where TSubscription : DbWebhookSubscription {
        private readonly WebhookSubscriptionBuilder<TSubscription> builder;
        
        internal EntityWebhookStorageBuilder(WebhookSubscriptionBuilder<TSubscription> builder, Type? resultType = null) {
			ResultType = resultType;
            this.builder = builder;

            AddDefaultStorage();
        }

		public Type? ResultType { get; private set; }

        /// <summary>
        /// Gets the <see cref="IServiceCollection"/> that is used to
        /// register the services for the storage.
        /// </summary>
        public IServiceCollection Services => builder.Services;

        private void AddDefaultStorage() {
            Services.TryAddScoped<IWebhookSubscriptionStore<TSubscription>, EntityWebhookSubscriptionStrore<TSubscription>>();
            Services.TryAddScoped<EntityWebhookSubscriptionStrore<TSubscription>>();

            if (typeof(TSubscription) == typeof(DbWebhookSubscription)) {
                Services.TryAddScoped<IWebhookSubscriptionStore<DbWebhookSubscription>, EntityWebhookSubscriptionStrore>();
                Services.TryAddScoped<EntityWebhookSubscriptionStrore<DbWebhookSubscription>>();
                Services.AddScoped<EntityWebhookSubscriptionStrore>();
            }

			if (ResultType != null && ResultType == typeof(DbWebhookDeliveryResult)) {
				Services.TryAddScoped<IWebhookDeliveryResultStore<DbWebhookDeliveryResult>, EntityWebhookDeliveryResultStore>();
				Services.AddScoped<EntityWebhookDeliveryResultStore>();
				Services.TryAddScoped<EntityWebhookDeliveryResultStore<DbWebhookDeliveryResult>>();
			}

			Services.TryAddSingleton(typeof(IDbWebhookConverter<>), typeof(DefaultDbWebhookConverter<>));
        }

        /// <summary>
        /// Registers the given type of DB context to be used for
        /// backing the storage of webhook subscriptions.
        /// </summary>
        /// <typeparam name="TContext">
        /// A type of <see cref="WebhookDbContext"/> that is used to store
        /// the webhook subscriptions.
        /// </typeparam>
        /// <param name="options">
        /// An optional action to configure the options of the context.
        /// </param>
        /// <param name="lifetime">
        /// An optional value that specifies the lifetime of the context.
        /// </param>
        /// <returns>
        /// Returns the current instance of the builder for chaining.
        /// </returns>
        public EntityWebhookStorageBuilder<TSubscription> UseContext<TContext>(Action<DbContextOptionsBuilder>? options = null, ServiceLifetime lifetime = ServiceLifetime.Scoped)
            where TContext : WebhookDbContext {
            if (typeof(TContext) != typeof(WebhookDbContext)) {
                Services.AddDbContext<WebhookDbContext, TContext>(options, lifetime);
            } else {
                Services.AddDbContext<WebhookDbContext>(options, lifetime);
            }

            return this;
        }

        /// <summary>
        /// Registers the default type of DB context to be used for
        /// backing the storage of webhook subscriptions.
        /// </summary>
        /// <param name="options">
        /// An optional action to configure the options of the context.
        /// </param>
        /// <param name="lifetime">
        /// An optional value that specifies the lifetime of the context.
        /// </param>
        /// <returns>
        /// Returns the current instance of the builder for chaining.
        /// </returns>
        public EntityWebhookStorageBuilder<TSubscription> UseContext(Action<DbContextOptionsBuilder>? options = null, ServiceLifetime lifetime = ServiceLifetime.Scoped)
            => UseContext<WebhookDbContext>(options, lifetime);

        /// <summary>
        /// Registers the given type of storage to be used for
        /// storing the webhook subscriptions.
        /// </summary>
        /// <typeparam name="TStore">
        /// The type of the storage to use for storing the webhook subscriptions,
        /// that is derived from <see cref="EntityWebhookSubscriptionStrore"/>.
        /// </typeparam>
        /// <returns>
        /// Returns the current instance of the builder for chaining.
        /// </returns>
        public EntityWebhookStorageBuilder<TSubscription> UseSubscriptionStore<TStore>()
            where TStore : EntityWebhookSubscriptionStrore<TSubscription> {
            Services.AddScoped<IWebhookSubscriptionStore<TSubscription>, TStore>();
            Services.AddScoped<TStore>();

            return this;
        }

		public EntityWebhookStorageBuilder<TSubscription> UseResultType(Type type) {
			if (type is null) 
				throw new ArgumentNullException(nameof(type));

			if (!typeof(DbWebhookDeliveryResult).IsAssignableFrom(type))
				throw new ArgumentException($"The given type '{type}' is not a valid result type");

			ResultType = type;

			return this;
		}

		public EntityWebhookStorageBuilder<TSubscription> UseResultStore(Type storeType) {
			if (ResultType == null)
				throw new InvalidOperationException("No result type was specified for the storage");

			var resultStoreType = typeof(IWebhookDeliveryResultStore<>).MakeGenericType(ResultType);

			if (!resultStoreType.IsAssignableFrom(storeType))
				throw new ArgumentException($"The given type '{storeType}' is not a valid result store");

			Services.AddScoped(resultStoreType, storeType);
			Services.AddScoped(storeType);

			return this;
		}
    }
}
