using Microsoft.Data.Sqlite;

namespace Deveel.Webhooks {
    public class SqliteTestDatabase : IDisposable {
        private readonly SqliteConnection connection;

        public SqliteTestDatabase() {
            connection = new SqliteConnection("Data Source=:memory:");
            connection.Open();
        }

        public string ConnectionString => connection.ConnectionString;

        public void Dispose() {
            connection.Close();
            connection.Dispose();
        }
    }
}
