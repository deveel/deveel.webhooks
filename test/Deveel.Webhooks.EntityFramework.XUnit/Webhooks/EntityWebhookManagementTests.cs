using Bogus;

using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace Deveel.Webhooks {
	public abstract class EntityWebhookManagementTests : WebhookManagementTestSuite<DbWebhookSubscription, string> {
        protected EntityWebhookManagementTests(ITestOutputHelper testOutput) : base(testOutput)
        {
        }

        protected override Faker<DbWebhookSubscription> Faker => new DbWebhookSubscriptionFaker();

		protected override string GenerateSubscriptionKey() => Guid.NewGuid().ToString();

		protected override async ValueTask InitializeAsync() {
			var options = Services!.GetRequiredService<DbContextOptions<WebhookDbContext>>();

			using var context = new WebhookDbContext(options);

			// await context.Database.EnsureDeletedAsync();
			await context.Database.EnsureCreatedAsync();

			await base.InitializeAsync();
		}

		protected override async ValueTask DisposeAsync() {
			var options = Services!.GetRequiredService<DbContextOptions<WebhookDbContext>>();

			using var context = new WebhookDbContext(options);
			context.Subscriptions.RemoveRange(context.Subscriptions);
			await context.SaveChangesAsync(true);

			//await context.Database.EnsureDeletedAsync();
		}
	}
}
