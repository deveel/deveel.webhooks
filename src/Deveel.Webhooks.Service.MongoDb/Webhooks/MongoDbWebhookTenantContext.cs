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

using Microsoft.Extensions.Options;

using MongoFramework;
using MongoFramework.Infrastructure;

namespace Deveel.Webhooks {
	/// <summary>
	/// Represents a multi-tenant MongoDB context that can be used to
	/// access to tenant-specific databases
	/// </summary>
	/// <typeparam name="TTenantInfo">
	/// The type of the tenant information.
	/// </typeparam>
    public class MongoDbWebhookTenantContext<TTenantInfo> : MongoDbTenantContext, IMongoDbWebhookContext
		where TTenantInfo : class, ITenantInfo, new() {
        
		/// <summary>
		/// Constructs the context with the given options and the current tenant
		/// retrieved from the provided <see cref="IMultiTenantContext{TTenantInfo}"/>.
		/// </summary>
		/// <param name="options">
		/// The set of options that are used to configure the connection to the store.
		/// </param>
		/// <param name="multiTenantContext">
		/// The context that provides access to the current tenant.
		/// </param>
		/// <exception cref="ArgumentNullException">
		/// Thrown when either the <paramref name="options"/> or the <paramref name="multiTenantContext"/>
		/// are <c>null</c>, or when it is not possible to retrieve the tenant information from the context.
		/// </exception>
        public MongoDbWebhookTenantContext(IOptions<MongoDbWebhookOptions> options, IMultiTenantContext<TTenantInfo> multiTenantContext) 
			: base(options.Value.BuildConnection(multiTenantContext?.TenantInfo?.ConnectionString ?? throw new ArgumentNullException()), multiTenantContext?.TenantInfo?.Id) {
            Options = options.Value;
        }

		/// <summary>
		/// Gets the set of options that are used to configure the connection to the store.
		/// </summary>
        public MongoDbWebhookOptions Options { get; }

        private void CheckAdd(object entity) {
			if (entity is MongoWebhookSubscription subscription) {
				if (String.IsNullOrWhiteSpace(subscription.TenantId)) {
					subscription.TenantId = TenantId;
				} else if (!String.Equals(TenantId, subscription.TenantId)) {
					throw new WebhookMongoException($"Subscription {subscription.Id} is not owned by tenant {TenantId}.");
				}
			}
			if (entity is MongoWebhookDeliveryResult deliveryResult) {
				deliveryResult.TenantId = TenantId;
			}
		}

		private void CheckDelete(object entity) {
			if (entity is MongoWebhookSubscription subscription) {
				if (!String.Equals(TenantId, subscription.TenantId))
					throw new WebhookMongoException($"Subscription {subscription.Id} is not owned by tenant {TenantId}.");
			} else if (entity is MongoWebhookDeliveryResult result) {
				if (!String.Equals(TenantId, result.TenantId))
					throw new WebhookMongoException($"Delivery result {result.Id} is not owned by tenant {TenantId}.");
			}
		}

		private void AttachEntity<TEntity>(TEntity entity) {
			if (entity is MongoWebhookSubscription subscription) {
				subscription.TenantId = TenantId;
			} else if (entity is MongoWebhookDeliveryResult result) {
				result.TenantId = TenantId;
			}
		}

		/// <inheritdoc/>
		protected override void AfterDetectChanges() {
			foreach (var entity in ChangeTracker.Entries()) {
				if (entity.State == EntityEntryState.Added) {
					CheckAdd(entity.Entity);
				} else if (entity.State == EntityEntryState.Deleted) {
					CheckDelete(entity.Entity);
				}
			}

			base.AfterDetectChanges();
		}

		/// <inheritdoc/>
		public override void Attach<TEntity>(TEntity entity) {
			AttachEntity(entity);

			base.Attach(entity);
		}

        /// <inheritdoc/>
        public override void AttachRange<TEntity>(IEnumerable<TEntity> entities) {
			foreach (var entity in entities) {
				AttachEntity(entity);
			}

			base.AttachRange(entities);
		}

		/// <inheritdoc/>
		protected override void OnConfigureMapping(MappingBuilder mappingBuilder) {
			mappingBuilder.Entity<MongoWebhookSubscription>();
			mappingBuilder.Entity<MongoWebhookDeliveryResult>();

			base.OnConfigureMapping(mappingBuilder);
		}
    }
}
