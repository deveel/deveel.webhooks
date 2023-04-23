using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using MongoFramework;

namespace Deveel.Webhooks {
	public class MongoDbWebhookContext : MongoDbContext, IMongoDbWebhookContext {
		public MongoDbWebhookContext(IMongoDbConnection connection) : base(connection) {
		}

		protected override void OnConfigureMapping(MappingBuilder mappingBuilder) {
			mappingBuilder.Entity<MongoWebhookSubscription>();
			mappingBuilder.Entity<MongoWebhookDeliveryResult>();

			base.OnConfigureMapping(mappingBuilder);
		}
	}
}
