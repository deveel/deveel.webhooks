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

using MongoFramework;
using MongoFramework.Infrastructure;

namespace Deveel.Webhooks {
    public class MongoDbWebhookTenantContext<TTenantInfo> : MongoDbTenantContext, IMongoDbWebhookContext
		where TTenantInfo : class, ITenantInfo, new() {
		public MongoDbWebhookTenantContext(IMultiTenantContext<TTenantInfo> multiTenantContext) 
			: base(BuildConnection(multiTenantContext?.TenantInfo), multiTenantContext?.TenantInfo?.Id) {
		}

		private static IMongoDbConnection BuildConnection(ITenantInfo? tenantInfo) {
			if (tenantInfo == null)
				throw new ArgumentNullException(nameof(tenantInfo));

			return MongoDbConnection.FromConnectionString(tenantInfo.ConnectionString);
		}

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

		public override void Attach<TEntity>(TEntity entity) {
			AttachEntity(entity);

			base.Attach(entity);
		}

		public override void AttachRange<TEntity>(IEnumerable<TEntity> entities) {
			foreach (var entity in entities) {
				AttachEntity(entity);
			}

			base.AttachRange(entities);
		}

		protected override void OnConfigureMapping(MappingBuilder mappingBuilder) {
			mappingBuilder.Entity<MongoWebhookSubscription>();
			mappingBuilder.Entity<MongoWebhookDeliveryResult>();

			base.OnConfigureMapping(mappingBuilder);
		}

		//public void CheckEntities(IEnumerable<IHaveTenantId> entities) {
		//	foreach (var entity in entities) {
		//		if (entity.TenantId != TenantId)
		//			throw new MongoFramework.MultiTenantException($"Entity type {entity.GetType().Name}, tenant ID does not match. Expected: {TenantId}, Entity has: {entity.TenantId}");
		//	}
		//}

		//public void CheckEntity(IHaveTenantId entity) {
		//	if (entity.TenantId != TenantId)
		//		throw new MongoFramework.MultiTenantException($"Entity type {entity.GetType().Name}, tenant ID does not match. Expected: {TenantId}, Entity has: {entity.TenantId}");
		//}

		//protected override void AfterDetectChanges() {
		//	ChangeTracker.EnforceMultiTenant(TenantId);
		//}

		//protected override WriteModelOptions GetWriteModelOptions() {
		//	return new WriteModelOptions { TenantId = TenantId };
		//}
	}
}
