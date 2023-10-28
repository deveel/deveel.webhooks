namespace Deveel.Webhooks {
	[CollectionDefinition(nameof(EntityWebhookManagementTestCollection))]
	public class EntityWebhookManagementTestCollection : ICollectionFixture<SqliteTestDatabase> {
	}
}
