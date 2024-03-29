using Bogus;

using Deveel.Data;

using Microsoft.Extensions.DependencyInjection;

using Xunit.Abstractions;

namespace Deveel.Webhooks {
	public class EntityDeliveryResultRepositoryTests : EntityWebhookTestBase {
        private readonly Faker<DbWebhookDeliveryResult> resultFaker;
		private readonly Faker<DbEventInfo> eventFaker;
        private List<DbWebhookDeliveryResult>? results;

        public EntityDeliveryResultRepositoryTests(SqliteTestDatabase sqlite, ITestOutputHelper outputHelper) : base(sqlite, outputHelper) {
			resultFaker = new DbWebhookDeliveryResultFaker();
			eventFaker = new DbEventInfoFaker();
        }

        private IWebhookDeliveryResultRepository<DbWebhookDeliveryResult,int> Repository
            => Services.GetRequiredService<IWebhookDeliveryResultRepository<DbWebhookDeliveryResult, int>>();

		public override async Task InitializeAsync() {
            await base.InitializeAsync();

			var events = eventFaker.Generate(10).ToList();
			results = new List<DbWebhookDeliveryResult>(10 * 5);

			foreach (var eventInfo in events) {
				var faker = new DbWebhookDeliveryResultFaker(eventInfo);
				var deliveryResults = resultFaker.Generate(5);
				results.AddRange(deliveryResults);
			}

			await Repository.AddRangeAsync(results);
        }

        private DbWebhookDeliveryResult NextRandom()
            => results![Random.Shared.Next(0, results.Count - 1)];

        [Fact]
        public async Task CreateNewResult() {
            var result = resultFaker.Generate();

            await Repository.AddAsync(result);

			Assert.NotNull(result.Id);
        }

        [Fact]
        public async Task GetExistingResult() {
            var result = NextRandom();

            var found = await Repository.FindAsync(result.Id!.Value);

            Assert.NotNull(found);
            Assert.Equal(result.Id, found.Id);

            var deliveryResult = Assert.IsAssignableFrom<IWebhookDeliveryResult>(found);

            Assert.Equal(result.Webhook.WebhookId, deliveryResult.Webhook.Id);
            Assert.Equal(result.Webhook.EventType, deliveryResult.Webhook.EventType);
            Assert.Equal(result.Webhook.TimeStamp, deliveryResult.Webhook.TimeStamp);
            Assert.Equal(result.EventInfo.EventType, deliveryResult.EventInfo.EventType);
            Assert.Equal(result.EventInfo.EventId, deliveryResult.EventInfo.Id);
            Assert.Equal(result.EventInfo.DataVersion, deliveryResult.EventInfo.DataVersion);
            Assert.Equal(result.EventInfo.Subject, deliveryResult.EventInfo.Subject);
            Assert.Equal(result.EventInfo.TimeStamp, deliveryResult.EventInfo.TimeStamp);
            Assert.Equal(result.DeliveryAttempts.Count, deliveryResult.DeliveryAttempts.Count());
        }

        [Fact]
        public async Task GetNotExistingResult() {
            var resultId = Random.Shared.Next(results!.Max(x => x.Id!.Value) + 1, Int32.MaxValue);

            var found = await Repository.FindAsync(resultId!);

            Assert.Null(found);
        }

        [Fact]
        public async Task RemoveExistingResult() {
            var result = NextRandom();

            var deleted = await Repository.RemoveAsync(result);

            Assert.True(deleted);

            var found = await Repository.FindAsync(result.Id!.Value);

            Assert.Null(found);
        }

        [Fact]
        public async Task RemoveNotExistingResult() {
            var resultId = Random.Shared.Next(results!.Max(x => x.Id!.Value) + 1, Int32.MaxValue);
            var result = resultFaker.Generate();
            result.Id = resultId;

            var removed = await Repository.RemoveAsync(result);

			Assert.False(removed);
        }

        [Fact]
        public async Task CountAll() {
            var count = await Repository.CountAllAsync();

            Assert.Equal(results!.Count, count);
        }

        [Fact]
        public async Task GetByWebhookId() {
            var result = NextRandom();

            var found = await Repository.FindByWebhookIdAsync(result.Webhook.WebhookId!, default);

            Assert.NotNull(found);
            Assert.Equal(result.Id, found.Id);
        }
    }
}
