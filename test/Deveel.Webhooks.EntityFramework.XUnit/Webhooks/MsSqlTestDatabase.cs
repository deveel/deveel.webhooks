using DotNet.Testcontainers.Builders;

using Microsoft.Data.SqlClient;

using Testcontainers.MsSql;

namespace Deveel.Webhooks
{
    public class MsSqlTestDatabase : IAsyncLifetime
    {
        private readonly MsSqlContainer _container;

        public MsSqlTestDatabase()
        {
            _container = new MsSqlBuilder()
				.WithImage("mcr.microsoft.com/mssql/server:2022-latest")
				.Build();
        }

        public string ConnectionString
        {
            get
            {
                var builder = new SqlConnectionStringBuilder(_container.GetConnectionString());
                builder.InitialCatalog = "webhooks_tests";
                return builder.ConnectionString;
            }
        }

        async ValueTask IAsyncDisposable.DisposeAsync()
        {
            await _container.DisposeAsync();
        }

        async ValueTask IAsyncLifetime.InitializeAsync()
        {
            await _container.StartAsync();
        }
    }
}
