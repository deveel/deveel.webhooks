using Microsoft.EntityFrameworkCore;

using Xunit.Abstractions;

namespace Deveel.Webhooks
{
    [Collection(nameof(SqliteTestCollection))]
    [Trait("DB", "SQLite")]
    public class SqliteDeliveryResultRepositoryTests : EntityDeliveryResultRepositoryTests
    {
        private readonly SqliteTestDatabase sqlite;

        public SqliteDeliveryResultRepositoryTests(SqliteTestDatabase sqlite, ITestOutputHelper outputHelper) : base(outputHelper)
        {
            this.sqlite = sqlite;
        }

        protected override void ConfigureWebhookEntityFramework(EntityWebhookStorageBuilder<DbWebhookSubscription> builder)
        {
            builder.UseContext(options => options.UseSqlite(sqlite.Connection));
        }
    }
}
