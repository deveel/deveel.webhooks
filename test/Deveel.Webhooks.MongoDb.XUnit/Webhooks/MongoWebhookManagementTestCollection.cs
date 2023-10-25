namespace Deveel.Webhooks {
	[CollectionDefinition(nameof(MongoWebhookManagementTestCollection))]
	public sealed class MongoWebhookManagementTestCollection : ICollectionFixture<MongoTestDatabase> {
	}
}
