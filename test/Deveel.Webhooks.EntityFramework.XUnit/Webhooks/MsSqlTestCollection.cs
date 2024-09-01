namespace Deveel.Webhooks
{
    [CollectionDefinition(nameof(MsSqlTestCollection))]
    public class MsSqlTestCollection : ICollectionFixture<MsSqlTestDatabase>
    {
    }
}
