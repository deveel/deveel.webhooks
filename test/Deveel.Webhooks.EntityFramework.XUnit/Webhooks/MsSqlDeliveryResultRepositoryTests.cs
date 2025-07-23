using Microsoft.EntityFrameworkCore;

using Xunit.Abstractions;

namespace Deveel.Webhooks
{
    [Collection(nameof(MsSqlTestCollection))]
    [Trait("DB", "SQLServer")]
    public class MsSqlDeliveryResultRepositoryTests : EntityDeliveryResultRepositoryTests
    {
        private readonly MsSqlTestDatabase sql;

        public MsSqlDeliveryResultRepositoryTests(MsSqlTestDatabase sql, ITestOutputHelper outputHelper) : base(outputHelper)
        {
            this.sql = sql;
        }

        protected override void ConfigureWebhookEntityFramework(EntityWebhookStorageBuilder<DbWebhookSubscription> builder)
        {
            builder.UseContext(x => x.UseSqlServer(sql.ConnectionString));
        }
    }
}
