using Deveel.Data;

using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Deveel.Webhooks {
	public static class StorageBuildingTests {
		[Fact]
		public static void UseDefaultRepository() {
			var services = new ServiceCollection();
			services.AddWebhookSubscriptions<DbWebhookSubscription>()
				.UseEntityFramework(ef => ef.UseContext(options => options.UseSqlite()));

			var provider = services.BuildServiceProvider();

			Assert.NotNull(provider.GetService<IWebhookSubscriptionRepository<DbWebhookSubscription>>());
			Assert.NotNull(provider.GetService<IRepository<DbWebhookSubscription>>());
			Assert.NotNull(provider.GetService<EntityWebhookSubscriptionRepository<DbWebhookSubscription>>());
			Assert.NotNull(provider.GetService<EntityWebhookSubscriptionRepository>());
		}

		[Fact]
		public static void UseCustomRepository() {
			var services = new ServiceCollection();
			services.AddWebhookSubscriptions<DbWebhookSubscription>()
				.UseEntityFramework(ef => ef
					.UseContext(options => options.UseSqlite())
					.UseSubscriptionRepository<MyWebhookSubscriptionRepository>());

			var provider = services.BuildServiceProvider();

			Assert.NotNull(provider.GetService<IWebhookSubscriptionRepository<DbWebhookSubscription>>());
			Assert.NotNull(provider.GetService<IRepository<DbWebhookSubscription>>());
			Assert.NotNull(provider.GetService<MyWebhookSubscriptionRepository>());
			Assert.NotNull(provider.GetService<EntityWebhookSubscriptionRepository<DbWebhookSubscription>>());
			Assert.Null(provider.GetService<EntityWebhookSubscriptionRepository>());

			var repository = provider.GetService<IWebhookSubscriptionRepository<DbWebhookSubscription>>();

			Assert.IsType<MyWebhookSubscriptionRepository>(repository);
		}

		class MyWebhookSubscriptionRepository : EntityWebhookSubscriptionRepository<DbWebhookSubscription> {
			public MyWebhookSubscriptionRepository(WebhookDbContext context, ILogger<EntityWebhookSubscriptionRepository<DbWebhookSubscription>>? logger = null) : base(context, logger) {
			}
		}
	}
}
