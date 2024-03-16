using Bogus;

using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

using Xunit.Abstractions;

namespace Deveel.Webhooks {
	[Collection(nameof(EntityWebhookManagementTestCollection))]
	public class EntityWebhookManagementTests : WebhookManagementTestSuite<DbWebhookSubscription, string> {
		private readonly SqliteTestDatabase sql;

		public EntityWebhookManagementTests(SqliteTestDatabase sql, ITestOutputHelper testOutput) : base(testOutput) {
			this.sql = sql;
		}

		protected override Faker<DbWebhookSubscription> Faker => new DbWebhookSubscriptionFaker();

		protected override string GenerateSubscriptionKey() => Guid.NewGuid().ToString();

		protected override void ConfigureWebhookStorage(WebhookSubscriptionBuilder<DbWebhookSubscription, string> options)
			=> options.UseEntityFramework(builder => builder.UseContext(ctx => ctx.UseSqlite(sql.Connection)));

		protected override async Task InitializeAsync() {
			var options = Services!.GetRequiredService<DbContextOptions<WebhookDbContext>>();

			using var context = new WebhookDbContext(options);

			await context.Database.EnsureDeletedAsync();
			await context.Database.EnsureCreatedAsync();

			await base.InitializeAsync();
		}

		protected override async Task DisposeAsync() {
			var options = Services!.GetRequiredService<DbContextOptions<WebhookDbContext>>();

			using var context = new WebhookDbContext(options);
			context.Subscriptions.RemoveRange(context.Subscriptions);
			await context.SaveChangesAsync(true);

			await context.Database.EnsureDeletedAsync();
		}
	}
}
