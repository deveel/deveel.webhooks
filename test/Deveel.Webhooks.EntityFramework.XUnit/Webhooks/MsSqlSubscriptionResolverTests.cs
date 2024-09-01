using Microsoft.EntityFrameworkCore;

using Xunit.Abstractions;

namespace Deveel.Webhooks
{
    [Collection(nameof(MsSqlTestCollection))]
    public class MsSqlSubscriptionResolverTests : EntitySubscriptionResolverTests
    {
        private readonly MsSqlTestDatabase sql;

        public MsSqlSubscriptionResolverTests(MsSqlTestDatabase sql, ITestOutputHelper outputHelper) : base(outputHelper)
        {
            this.sql = sql;
        }

        protected override void ConfigureWebhookEntityFramework(EntityWebhookStorageBuilder<DbWebhookSubscription> builder)
        {
            builder.UseContext(options => options.UseSqlServer(sql.ConnectionString));
        }
    }
}
