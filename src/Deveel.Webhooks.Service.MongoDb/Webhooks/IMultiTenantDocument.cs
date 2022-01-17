using System;

namespace Deveel.Webhooks {
	interface IMultiTenantDocument : IMongoDocument {
		string TenantId { get; set; }
	}
}
