using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xunit.Abstractions;

namespace Deveel.Webhooks
{
    [Collection(nameof(SqliteTestCollection))]
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
