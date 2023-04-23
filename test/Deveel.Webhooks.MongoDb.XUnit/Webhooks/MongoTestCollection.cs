namespace Deveel.Webhooks {
	[CollectionDefinition(nameof(MongoTestCollection), DisableParallelization = true)]
	public sealed class MongoTestCollection : ICollectionFixture<MongoTestCluster> {
	}
}
