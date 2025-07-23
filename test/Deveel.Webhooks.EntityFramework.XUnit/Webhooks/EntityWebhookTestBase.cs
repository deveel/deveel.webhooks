using System.Net;

using Deveel.Util;

using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

using Xunit.Abstractions;

namespace Deveel.Webhooks {
    public abstract class EntityWebhookTestBase : IAsyncLifetime {
        private readonly ITestOutputHelper outputHelper;

        protected EntityWebhookTestBase(ITestOutputHelper outputHelper) {
            this.outputHelper = outputHelper;
        }

        protected IServiceProvider Services { get; private set; }

        private IServiceProvider BuildServiceProvider() {
            var services = new ServiceCollection();

            services.AddWebhookSubscriptions<DbWebhookSubscription, string>(builder => ConfigureWebhookService(builder))
                .AddTestHttpClient(OnRequestAsync)
                .AddLogging(logging => logging.AddXUnit(outputHelper));

			services.AddWebhookNotifier<Webhook>(builder => builder.UseEntityFrameworkDeliveryResults());

            return services.BuildServiceProvider();
        }

        protected virtual Task<HttpResponseMessage> OnRequestAsync(HttpRequestMessage message) {
            return Task.FromResult(new HttpResponseMessage(HttpStatusCode.OK));
        }

        protected virtual void ConfigureWebhookService(WebhookSubscriptionBuilder<DbWebhookSubscription, string> builder) {
            builder.UseEntityFramework(ConfigureWebhookEntityFramework);
        }

		protected virtual void ConfigureWebhookEntityFramework(EntityWebhookStorageBuilder<DbWebhookSubscription> builder) {
		}

        protected virtual void ConfigureServices(IServiceCollection services) {
            
        }

        public virtual async Task DisposeAsync() {
            var options = Services.GetRequiredService<DbContextOptions<WebhookDbContext>>();

            using var context = new WebhookDbContext(options);
            await context.Database.EnsureDeletedAsync();
        }

        public virtual async Task InitializeAsync() {
            Services = BuildServiceProvider();

            var options = Services.GetRequiredService<DbContextOptions<WebhookDbContext>>();
            using var context = new WebhookDbContext(options);

            await context.Database.EnsureCreatedAsync();
        }
    }
}
