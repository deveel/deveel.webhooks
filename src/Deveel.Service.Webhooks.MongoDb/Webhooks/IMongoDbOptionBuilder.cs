using System;

namespace Deveel.Webhooks {
	public interface IMongoDbOptionBuilder {
		IMongoDbOptionBuilder SetConnectionString(string connectionString);

		IMongoDbOptionBuilder SetDatabaseName(string databaseName);

		IMongoDbOptionBuilder SetSubscriptionsCollection(string collectionName);

		IMongoDbOptionBuilder SetWebhooksCollection(string collectionName);

		IMongoDbOptionBuilder SetMultiTenancyHandling(MongoDbMultiTenancyHandling handling);

		IMongoDbOptionBuilder SetTenantFieldName(string fieldName);

		IMongoDbOptionBuilder SetTenantDatabaseFormat(string format);

		IMongoDbOptionBuilder SetTenantCollectionFormat(string format);
	}
}
