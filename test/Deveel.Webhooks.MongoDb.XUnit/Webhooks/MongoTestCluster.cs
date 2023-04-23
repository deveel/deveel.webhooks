using Microsoft.Extensions.Logging.Abstractions;

using Mongo2Go;

namespace Deveel.Webhooks {
	public class MongoTestCluster : IDisposable {
		private readonly MongoDbRunner mongo;

		public MongoTestCluster() {
			mongo = MongoDbRunner.Start(logger: NullLogger.Instance);
		}

		public string ConnectionString => mongo.ConnectionString;

		public void Dispose() {
			mongo.Dispose();
		}
	}
}
