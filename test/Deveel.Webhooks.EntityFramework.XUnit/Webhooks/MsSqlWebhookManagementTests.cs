using Microsoft.EntityFrameworkCore;

using Xunit.Abstractions;

namespace Deveel.Webhooks
{
    [Collection(nameof(MsSqlTestCollection))]
    [Trait("DB", "SQLServer")]
    public class MsSqlWebhookManagementTests : EntityWebhookManagementTests
    {
        private readonly MsSqlTestDatabase sql;

        public MsSqlWebhookManagementTests(MsSqlTestDatabase sql, ITestOutputHelper testOutput) : base(testOutput)
        {
            this.sql = sql;
        }

        protected override void ConfigureWebhookStorage(WebhookSubscriptionBuilder<DbWebhookSubscription, string> options)
            => options.UseEntityFramework(builder => builder.UseContext(ctx => ctx.UseSqlServer(sql.ConnectionString, s => s.EnableRetryOnFailure())));
    }
}
