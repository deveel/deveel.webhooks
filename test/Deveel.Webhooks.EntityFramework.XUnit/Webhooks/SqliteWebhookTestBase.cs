using Microsoft.EntityFrameworkCore;

using Xunit.Abstractions;

namespace Deveel.Webhooks
{
    [Collection(nameof(EntityTestCollection))]
    public class SqliteWebhookTestBase : EntityWebhookTestBase
    {
        private readonly SqliteTestDatabase sqlite;

        public SqliteWebhookTestBase(SqliteTestDatabase sqlite, ITestOutputHelper outputHelper)
            : base(outputHelper)
        {
            this.sqlite = sqlite;
        }

        protected override void ConfigureWebhookEntityFramework(EntityWebhookStorageBuilder<DbWebhookSubscription> builder)
        {
            builder.UseContext(options => options.UseSqlite(sqlite.ConnectionString));
        }
    }
}
