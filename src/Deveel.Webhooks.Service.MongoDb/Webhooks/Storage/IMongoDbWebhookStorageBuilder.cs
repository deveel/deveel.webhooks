using System;
using System.Collections.Generic;
using System.Text;

using Deveel.Data;

namespace Deveel.Webhooks.Storage {
	public interface IMongoDbWebhookStorageBuilder {
		IMongoDbWebhookStorageBuilder Configure(string sectionName, string connectionStringName = null);

		IMongoDbWebhookStorageBuilder Configure(Action<IMongoDbOptionBuilder> configure);

		IMongoDbWebhookStorageBuilder Configure(Action<MongoDbOptions> configure);

		IMongoDbWebhookStorageBuilder UseSubscriptionStore<TStore>()
			where TStore : MongoDbWebhookSubscriptionStrore;

		IMongoDbWebhookStorageBuilder UseSubscriptionStoreProvider<TProvider>()
			where TProvider : MongoDbWebhookSubscriptionStoreProvider;
	}
}
