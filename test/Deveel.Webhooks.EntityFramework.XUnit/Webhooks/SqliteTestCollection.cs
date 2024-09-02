namespace Deveel.Webhooks {
	[CollectionDefinition(nameof(SqliteTestCollection))]
	public class SqliteTestCollection : ICollectionFixture<SqliteTestDatabase> {
	}
}
