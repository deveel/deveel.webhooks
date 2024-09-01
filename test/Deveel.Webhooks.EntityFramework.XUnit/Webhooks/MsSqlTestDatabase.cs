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
                .WithImage("mcr.microsoft.com/mssql/server:2019-CU14-ubuntu-20.04")
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

        async Task IAsyncLifetime.DisposeAsync()
        {
            await _container.DisposeAsync();
        }

        async Task IAsyncLifetime.InitializeAsync()
        {
            await _container.StartAsync();
        }
    }
}
