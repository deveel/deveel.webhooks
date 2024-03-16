using Bogus;

using Deveel.Data;

using Microsoft.Extensions.DependencyInjection;

using MongoDB.Bson;

using Xunit.Abstractions;

namespace Deveel.Webhooks {
    public class MongoDbDeliveryResultStoreTests : MongoDbWebhookTestBase {
        private readonly Faker<MongoWebhookDeliveryResult> faker;
        private readonly List<MongoWebhookDeliveryResult> results;

        public MongoDbDeliveryResultStoreTests(MongoTestDatabase mongo, ITestOutputHelper outputHelper) : base(mongo, outputHelper) {
            var receiver = new Faker<MongoWebhookReceiver>()
                .RuleFor(x => x.BodyFormat, f => f.Random.ListItem(new[] {"json","xml"}))
                .RuleFor(x => x.DestinationUrl, f => f.Internet.Url())
                .RuleFor(x => x.SubscriptionId, f => f.Random.Guid().OrNull(f)?.ToString())
                .RuleFor(x => x.SubscriptionName, f => f.Name.JobType());

            var webhook = new Faker<MongoWebhook>()
                .RuleFor(x => x.EventType, f => f.Random.ListItem(new[] { "data.created", "data.deleted", "data.updated" }))
                .RuleFor(x => x.WebhookId, f => f.Random.Guid().ToString())
                .RuleFor(x => x.TimeStamp, f => f.Date.PastOffset(1))
                .RuleFor(x => x.Data, new BsonDocument { { "data-type", "test" } });

            var attempt = new Faker<MongoWebhookDeliveryAttempt>()
                .RuleFor(x => x.ResponseStatusCode, f => f.Random.ListItem(new int?[] { 200, 201, 204, 400, 404, 500, null }))
                .RuleFor(x => x.StartedAt, (f, a) => f.Date.PastOffset())
                .RuleFor(x => x.EndedAt, (f, a) => a.StartedAt.AddMilliseconds(200).OrNull(f));

			var eventInfo = new Faker<MongoEventInfo>()
				.RuleFor(x => x.EventType, f => f.Random.ListItem(new[] { "created", "deleted", "updated" }))
				.RuleFor(x => x.DataVersion, "1.0")
				.RuleFor(x => x.EventId, f => f.Random.Guid().ToString("N"))
				.RuleFor(x => x.TimeStamp, f => f.Date.PastOffset())
				.RuleFor(x => x.Subject, "data")
				.RuleFor(x => x.EventData, f => new BsonDocument {
					{ "data-type", f.Random.Word() },
					{ "users", new BsonArray(f.Random.ListItems(new string[]{ "user1", "user2" })) }
				});

            faker = new Faker<MongoWebhookDeliveryResult>()
				.RuleFor(x => x.EventInfo, f => eventInfo.Generate())
                .RuleFor(x => x.Receiver, f => receiver.Generate())
                .RuleFor(x => x.Webhook, f => webhook.Generate())
                .RuleFor(x => x.DeliveryAttempts, f => attempt.Generate(2));

            results = new List<MongoWebhookDeliveryResult>();
        }

        private IWebhookDeliveryResultRepository<MongoWebhookDeliveryResult, ObjectId> Repository
            => ScopeServices.GetRequiredService<IWebhookDeliveryResultRepository<MongoWebhookDeliveryResult, ObjectId>>();

        public override async Task InitializeAsync() {
            await base.InitializeAsync();

            var fakes = faker.Generate(112).ToList();

            foreach (var attempt in fakes) {
                await CreateAttemptAsync(attempt);

                results.Add(attempt);
            }
        }

        private Task CreateAttemptAsync(MongoWebhookDeliveryResult attempt) {
            return Repository.AddAsync(attempt, default);
        }

        private MongoWebhookDeliveryResult NextRandom()
            => results[Random.Shared.Next(0, results.Count - 1)];

        [Fact]
        public async Task AddNewResult() {
            var result = faker.Generate();

            await Repository.AddAsync(result);

            Assert.NotEqual(ObjectId.Empty, result.Id);

			// TODO: access the low-level database to get the document
        }

        [Fact]
        public async Task GetExistingResult() {
            var result = NextRandom();

            var found = await Repository.FindAsync(result.Id);

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
            var resultId = ObjectId.GenerateNewId();

            var found = await Repository.FindAsync(resultId);

            Assert.Null(found);
        }

        [Fact]
        public async Task RemoveExistingResult() {
            var result = NextRandom();

            var removed = await Repository.RemoveAsync(result);

            Assert.True(removed);

            var found = await Repository.FindAsync(result.Id);

            Assert.Null(found);
        }

        [Fact]
        public async Task RemoveNotExistingResult() {
            var result = faker.Generate();

            var removed = await Repository.RemoveAsync(result);
			Assert.False(removed);
            
            var found = await Repository.FindAsync(result.Id);
            Assert.Null(found);
        }

        [Fact]
        public async Task CountAll() {
            var count = await Repository.CountAllAsync();

            Assert.Equal(results.Count, count);
        }

        [Fact]
        public async Task GetByWebhookId() {
            var result = NextRandom();

            var found = await Repository.FindByWebhookIdAsync(result.Webhook.WebhookId, default);

            Assert.NotNull(found);
            Assert.Equal(result.Id, found.Id);
        }
    }
}
