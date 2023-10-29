using Deveel.Data;

using Microsoft.Extensions.DependencyInjection;

using Xunit.Abstractions;

using static System.Formats.Asn1.AsnWriter;

namespace Deveel.Webhooks {
	public class EntitySubscriptionResolverTests : EntityWebhookTestBase {
		public EntitySubscriptionResolverTests(SqliteTestDatabase sqlite, ITestOutputHelper outputHelper) 
			: base(sqlite, outputHelper) {
		}

		protected IList<DbWebhookSubscription> Subscriptions { get; private set; }

		protected IRepository<DbWebhookSubscription> Repository => Services.GetRequiredService<IRepository<DbWebhookSubscription>>();

		public IWebhookSubscriptionResolver Resolver => Services.GetRequiredService<IWebhookSubscriptionResolver>();

		public override async Task InitializeAsync() {
			await base.InitializeAsync();

			Subscriptions = new DbWebhookSubscriptionFaker().Generate(102);

			await Repository.AddRangeAsync(Subscriptions);
		}

		public override async Task DisposeAsync() {
			await Repository.RemoveRangeAsync(Subscriptions);

			await base.DisposeAsync();
		}

		private DbWebhookSubscription Random(Func<DbWebhookSubscription, bool>? predicate = null, int maxRetries = 100) {
			while (maxRetries-- >= 0) {
				var index = System.Random.Shared.Next(0, Subscriptions.Count - 1);
				var subscription = Subscriptions[index];

				if (predicate == null || predicate(subscription))
					return subscription;
			}

			throw new InvalidOperationException("Could not find a random subscription");
		}

		[Fact]
		public async Task ResolveActiveSubscriptions() {
			var subscription = Random(x => x.Events.Any());
			var eventType = subscription.Events[0].EventType;

			var subCount = Subscriptions.Count(x => x.Events.Any(y => y.EventType == eventType) &&
											   x.Status == WebhookSubscriptionStatus.Active);

			var subscriptions = await Resolver.ResolveSubscriptionsAsync(eventType, true, CancellationToken.None);

			Assert.NotNull(subscriptions);
			Assert.NotEmpty(subscriptions);

			Assert.Equal(subCount, subscriptions.Count);
		}

		[Fact]
		public async Task ResolveAllSubscriptions() {
			var subscription = Random(x => x.Events.Any());
			var eventType = subscription.Events[0].EventType;

			var subCount = Subscriptions.Count(x => x.Events.Any(y => y.EventType == eventType));

			var subscriptions = await Resolver.ResolveSubscriptionsAsync(eventType, false, CancellationToken.None);

			Assert.NotNull(subscriptions);
			Assert.NotEmpty(subscriptions);

			Assert.Equal(subCount, subscriptions.Count);
		}
	}
}
