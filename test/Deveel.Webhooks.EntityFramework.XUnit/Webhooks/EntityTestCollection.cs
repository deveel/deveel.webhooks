namespace Deveel.Webhooks {
    [CollectionDefinition(nameof(EntityTestCollection))]
    public class EntityTestCollection : ICollectionFixture<SqliteTestDatabase> {
    }
}
