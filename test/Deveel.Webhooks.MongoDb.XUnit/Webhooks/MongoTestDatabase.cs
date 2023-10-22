using MongoDB.Driver;

using Testcontainers.MongoDb;

namespace Deveel.Webhooks {
	public class MongoTestDatabase : IAsyncLifetime, IDisposable {
		private readonly MongoDbContainer container;
		private bool disposed;

		public MongoTestDatabase() {
			container = new MongoDbBuilder()
				.WithUsername("")
				.WithPassword("")
				.WithPortBinding(27017, true)
				.Build();
		}

		public string ConnectionString => container.GetConnectionString();

		public string GetConnectionString(string databaseName = "testdb") {
			var connectionString = container.GetConnectionString();

			if (!String.IsNullOrWhiteSpace(databaseName)) {
				var urlBuilder = new MongoUrlBuilder(connectionString);
				urlBuilder.DatabaseName = databaseName;
				connectionString = urlBuilder.ToString();
			}

			return connectionString;
		}

		public async Task InitializeAsync() {
			await container.StartAsync();
		}

		public async Task DisposeAsync() {
			if (!disposed) {
				await container.StopAsync();
				await container.DisposeAsync();
				disposed = true;
			}
		}

		public void Dispose() {
			DisposeAsync().ConfigureAwait(false).GetAwaiter().GetResult();
		}
	}
}
