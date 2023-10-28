using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Deveel.Webhooks {
	[CollectionDefinition(nameof(MongoTestCollection))]
	public sealed class MongoTestCollection : ICollectionFixture<MongoTestDatabase> {
	}
}
