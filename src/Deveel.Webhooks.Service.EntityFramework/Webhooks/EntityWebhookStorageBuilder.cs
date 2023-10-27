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

using Deveel.Data;

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

		/// <summary>
		/// Gets the entity type to be used to store the results of 
		/// webhook deliveries in the database.
		/// </summary>
		public Type? ResultType { get; private set; }

        /// <summary>
        /// Gets the <see cref="IServiceCollection"/> that is used to
        /// register the services for the storage.
        /// </summary>
        public IServiceCollection Services => builder.Services;

        private void AddDefaultStorage() {
			Services.AddRepository<EntityWebhookSubscriptionRepository<TSubscription>>();

            if (typeof(TSubscription) == typeof(DbWebhookSubscription)) {
				Services.AddRepository<EntityWebhookSubscriptionRepository>();
            }

			if (ResultType != null) {
				var resultStoreType = typeof(EntityWebhookDeliveryResultRepository<>).MakeGenericType(ResultType);
				Services.AddRepository(resultStoreType);

				if (ResultType == typeof(DbWebhookDeliveryResult))
					Services.AddRepository<EntityWebhookDeliveryResultRepository>();
			}
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
		/// Registers the given type of DB context to be used for
		/// backing the storage of webhook subscriptions.
		/// </summary>
		/// <typeparam name="TContext">
		/// The type of the <see cref="WebhookDbContext"/> to use.
		/// </typeparam>
		/// <param name="options">
		/// A configuration action that receives an instance of <see cref="ITenantInfo"/>
		/// that can be used to configure the options of the context.
		/// </param>
		/// <param name="lifetime">
		/// An optional value that specifies the lifetime of the context.
		/// </param>
		/// <returns>
		/// Returns the current instance of the builder for chaining.
		/// </returns>
		public EntityWebhookStorageBuilder<TSubscription> UseContext<TContext>(Action<ITenantInfo?, DbContextOptionsBuilder> options, ServiceLifetime lifetime = ServiceLifetime.Scoped) 
			where TContext : WebhookDbContext {
			var factory = (IServiceProvider sp, DbContextOptionsBuilder builder) => {
				var tenantInfo = sp.GetService<IMultiTenantContext>()?.TenantInfo;
				options(tenantInfo, builder);
			};

			if (typeof(TContext) != typeof(WebhookDbContext)) {
				Services.AddDbContext<WebhookDbContext, TContext>(factory, lifetime);
			} else {
				Services.AddDbContext<WebhookDbContext>(factory, lifetime);
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
        /// <typeparam name="TRepository">
        /// The type of the storage to use for storing the webhook subscriptions,
        /// that is derived from <see cref="EntityWebhookSubscriptionRepository"/>.
        /// </typeparam>
        /// <returns>
        /// Returns the current instance of the builder for chaining.
        /// </returns>
        public EntityWebhookStorageBuilder<TSubscription> UseSubscriptionRepository<TRepository>()
            where TRepository : EntityWebhookSubscriptionRepository<TSubscription> {

			Services.RemoveAll<IRepository<TSubscription>>();
			Services.RemoveAll<IPageableRepository<TSubscription>>();
			Services.RemoveAll<IQueryableRepository<TRepository>>();
			Services.RemoveAll<IWebhookSubscriptionRepository<TSubscription>>();
			Services.RemoveAll<EntityWebhookSubscriptionRepository<TSubscription>>();
			Services.RemoveAll<EntityWebhookSubscriptionRepository>();

			Services.AddRepository<TRepository>();

			if (typeof(EntityWebhookSubscriptionRepository<TSubscription>) != typeof(TRepository))
				Services.AddScoped<EntityWebhookSubscriptionRepository<TSubscription>, TRepository>();

            return this;
        }

		/// <summary>
		/// Configures the storage to use the given type of result.
		/// </summary>
		/// <param name="type">
		/// The type of the result to use, that must be derived from
		/// the <see cref="DbWebhookDeliveryResult"/> type.
		/// </param>
		/// <returns>
		/// Returns the current instance of the builder for chaining.
		/// </returns>
		/// <exception cref="ArgumentNullException">
		/// Thrown when the given <paramref name="type"/> is <c>null</c>.
		/// </exception>
		/// <exception cref="ArgumentException">
		/// Thrown when the given <paramref name="type"/> is not derived from
		/// <see cref="DbWebhookDeliveryResult"/>.
		/// </exception>
		public EntityWebhookStorageBuilder<TSubscription> UseResultType(Type type) {
			ArgumentNullException.ThrowIfNull(type, nameof(type));

			if (!typeof(DbWebhookDeliveryResult).IsAssignableFrom(type))
				throw new ArgumentException($"The given type '{type}' is not a valid result type");

			ResultType = type;

			return this;
		}

		public EntityWebhookStorageBuilder<TSubscription> UseResult<TResult>()
			where TResult : DbWebhookDeliveryResult
			=> UseResultType(typeof(TResult));

		public EntityWebhookStorageBuilder<TSubscription> UseResultRepository(Type repositoryType) {
			if (ResultType == null)
				throw new InvalidOperationException("No result type was specified for the storage");

			var resultStoreType = typeof(IWebhookDeliveryResultRepository<>).MakeGenericType(ResultType);

			if (!resultStoreType.IsAssignableFrom(repositoryType))
				throw new ArgumentException($"The given type '{repositoryType}' is not a valid result store");

			Services.AddRepository(resultStoreType);

			return this;
		}
    }
}
