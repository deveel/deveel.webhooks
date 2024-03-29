using Deveel.Data;

using Microsoft.Extensions.DependencyInjection;

using MongoDB.Bson;
using MongoDB.Driver;

using Xunit.Abstractions;

namespace Deveel.Webhooks {
	public class MongoSubsctiptionResolverTests : MongoDbWebhookTestBase {
		public MongoSubsctiptionResolverTests(MongoTestDatabase mongo, ITestOutputHelper outputHelper) 
			: base(mongo, outputHelper) {
		}

		protected IList<MongoWebhookSubscription> Subscriptions { get; private set; }

		protected IRepository<MongoWebhookSubscription, ObjectId> Repository 
			=> Scope.ServiceProvider.GetRequiredService<IRepository<MongoWebhookSubscription, ObjectId>>();

		public IWebhookSubscriptionResolver Resolver => Scope.ServiceProvider.GetRequiredService<IWebhookSubscriptionResolver>();

		public override async Task InitializeAsync() {
			await base.InitializeAsync();

			Subscriptions = new MongoWebhookSubscriptionFaker().Generate(102);

			await Repository.AddRangeAsync(Subscriptions);
		}

		public override async Task DisposeAsync() {
			await Repository.RemoveRangeAsync(Subscriptions);

			await base.DisposeAsync();
		}

		private MongoWebhookSubscription Random(Func<MongoWebhookSubscription, bool>? predicate = null, int maxRetries = 100) {
			while(maxRetries-- >= 0) {
				var index = System.Random.Shared.Next(0, Subscriptions.Count - 1);
				var subscription = Subscriptions[index];

				if (predicate == null || predicate(subscription))
					return subscription;
			}

			throw new InvalidOperationException("Could not find a random subscription");
		}

		[Fact]
		public async Task ResolveActiveSubscriptions() {
			var subscription = Random(x => x.EventTypes.Any());
			var eventType = subscription.EventTypes[0];

			var subCount = Subscriptions.Count(x => x.EventTypes.Any(y => y == eventType) && 
											   x.Status == WebhookSubscriptionStatus.Active);

			var subscriptions = await Resolver.ResolveSubscriptionsAsync(eventType, true, CancellationToken.None);

			Assert.NotNull(subscriptions);
			Assert.NotEmpty(subscriptions);

			Assert.Equal(subCount, subscriptions.Count);
		}

		[Fact]
		public async Task ResolveAllSubscriptions() {
			var subscription = Random(x => x.EventTypes.Any());
			var eventType = subscription.EventTypes[0];

			var subCount = Subscriptions.Count(x => x.EventTypes.Any(y => y == eventType));

			var subscriptions = await Resolver.ResolveSubscriptionsAsync(eventType, false, CancellationToken.None);

			Assert.NotNull(subscriptions);
			Assert.NotEmpty(subscriptions);

			Assert.Equal(subCount, subscriptions.Count);
		}
	}
}
