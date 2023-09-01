using System.Net;

using Deveel.Util;

using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

using Xunit.Abstractions;

namespace Deveel.Webhooks {
    [Collection(nameof(EntityTestCollection))]
    public abstract class EntityWebhookTestBase : IAsyncLifetime {
        private readonly SqliteTestDatabase sqlite;
        private readonly ITestOutputHelper outputHelper;

        protected EntityWebhookTestBase(SqliteTestDatabase sqlite, ITestOutputHelper outputHelper) {
            this.sqlite = sqlite;
            this.outputHelper = outputHelper;
            Services = BuildServiceProvider();
        }

        protected IServiceProvider Services { get; }

        private IServiceProvider BuildServiceProvider() {
            var services = new ServiceCollection();

            services.AddWebhookSubscriptions<WebhookSubscriptionEntity>(builder => ConfigureWebhookService(builder))
                .AddTestHttpClient(OnRequestAsync)
                .AddLogging(logging => logging.AddXUnit(outputHelper));

            return services.BuildServiceProvider();
        }

        protected virtual Task<HttpResponseMessage> OnRequestAsync(HttpRequestMessage message) {
            return Task.FromResult(new HttpResponseMessage(HttpStatusCode.OK));
        }

        protected virtual void ConfigureWebhookService(WebhookSubscriptionBuilder<WebhookSubscriptionEntity> builder) {
            builder.UseEntityFramework(ef => ef.UseContext(options => options.UseSqlite(sqlite.ConnectionString)));
        }

        protected virtual void ConfigureServices(IServiceCollection services) {
            
        }

        public virtual async Task DisposeAsync() {
            var context = Services.GetService<WebhookDbContext>()!;
            await context.Database.EnsureDeletedAsync();

            if (context.Database.GetDbConnection().State == System.Data.ConnectionState.Open)
                await context.Database.CloseConnectionAsync();
        }

        public virtual async Task InitializeAsync() {
            var context = Services.GetService<WebhookDbContext>()!;
            if (context.Database.GetDbConnection().State == System.Data.ConnectionState.Closed)
                await context.Database.OpenConnectionAsync();

            // await context.Database.EnsureDeletedAsync();
            await context.Database.EnsureCreatedAsync();
        }
    }
}
