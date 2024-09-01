using Microsoft.EntityFrameworkCore;

using Xunit.Abstractions;

namespace Deveel.Webhooks
{
    [Collection(nameof(SqliteTestCollection))]
    public class SqliteWebhookManagementTests : EntityWebhookManagementTests
    {
        private readonly SqliteTestDatabase sql;

        public SqliteWebhookManagementTests(SqliteTestDatabase sql, ITestOutputHelper testOutput) : base(testOutput)
        {
            this.sql = sql;
        }

        protected override void ConfigureWebhookStorage(WebhookSubscriptionBuilder<DbWebhookSubscription, string> options)
            => options.UseEntityFramework(builder => builder.UseContext(ctx => ctx.UseSqlite(sql.Connection)));
    }
}
