using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Deveel.Webhooks {
	[CollectionDefinition(nameof(MongoWebhookManagementTestCollection))]
	public sealed class MongoWebhookManagementTestCollection : ICollectionFixture<MongoTestDatabase> {
	}
}
