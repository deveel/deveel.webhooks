using System.Collections.Generic;

using Finbuckle.MultiTenant;

using MongoFramework;
using MongoFramework.Infrastructure.Commands;

namespace Deveel.Webhooks {
	public class MongoDbWebhookTenantContext : MongoDbWebhookContext, IMongoDbTenantContext {
		public MongoDbWebhookTenantContext(IMultiTenantContext multiTenantContext) 
			: base(BuildConnection(multiTenantContext?.TenantInfo)) {
			TenantInfo = multiTenantContext?.TenantInfo;
		}

		private static IMongoDbConnection BuildConnection(ITenantInfo tenantInfo)
			=> MongoDbConnection.FromConnectionString(tenantInfo.ConnectionString);

		public ITenantInfo TenantInfo { get; }

		public string TenantId => TenantInfo?.Id;

		private void SetTenantId<TEntity>(TEntity entity) {
			if (entity is MongoWebhookSubscription subscription)
				subscription.TenantId = TenantId;
			if (entity is MongoWebhookDeliveryResult deliveryResult)
				deliveryResult.TenantId = TenantId;
		}

		public override void Attach<TEntity>(TEntity entity) {
			SetTenantId<TEntity>(entity);

			base.Attach(entity);
		}

		public override void AttachRange<TEntity>(IEnumerable<TEntity> entities) {
			foreach (var entity in entities) {
				SetTenantId<TEntity>(entity);
			}

			base.AttachRange(entities);
		}

		public void CheckEntities(IEnumerable<IHaveTenantId> entities) {
			foreach (var entity in entities) {
				if (entity.TenantId != TenantId)
					throw new MongoFramework.MultiTenantException($"Entity type {entity.GetType().Name}, tenant ID does not match. Expected: {TenantId}, Entity has: {entity.TenantId}");
			}
		}

		public void CheckEntity(IHaveTenantId entity) {
			if (entity.TenantId != TenantId)
				throw new MongoFramework.MultiTenantException($"Entity type {entity.GetType().Name}, tenant ID does not match. Expected: {TenantId}, Entity has: {entity.TenantId}");
		}

		protected override void AfterDetectChanges() {
			ChangeTracker.EnforceMultiTenant(TenantId);
		}

		protected override WriteModelOptions GetWriteModelOptions() {
			return new WriteModelOptions { TenantId = TenantId };
		}
	}
}
