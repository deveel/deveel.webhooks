using Microsoft.Data.Sqlite;

namespace Deveel.Webhooks {
	public class SqliteTestDatabase : IDisposable {
		public SqliteTestDatabase() {
            Connection = new SqliteConnection("DataSource=:memory:");
			if (Connection.State != System.Data.ConnectionState.Open)
				Connection.Open();

			Connection.EnableExtensions();
        }

        public string ConnectionString => Connection.ConnectionString;

		public SqliteConnection Connection { get; }

		public void Dispose() {
            Connection?.Close();
            Connection?.Dispose();
        }
    }
}
