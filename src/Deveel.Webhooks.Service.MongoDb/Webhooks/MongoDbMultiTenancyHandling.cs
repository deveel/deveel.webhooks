using System;

namespace Deveel.Webhooks {
	public enum MongoDbMultiTenancyHandling {
		TenantDatabase,
		TenantCollection,
		TenantField
	}
}
