using Bogus;

using Microsoft.Extensions.DependencyInjection;

using MongoDB.Bson;

using Xunit.Abstractions;

namespace Deveel.Webhooks {
    public class MongoDbDeliveryResultStoreTests : MongoDbWebhookTestBase {
        private readonly Faker<MongoWebhookDeliveryResult> faker;
        private readonly List<MongoWebhookDeliveryResult> results;

        public MongoDbDeliveryResultStoreTests(MongoTestCluster mongo, ITestOutputHelper outputHelper) : base(mongo, outputHelper) {
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

            faker = new Faker<MongoWebhookDeliveryResult>()
                .RuleFor(x => x.Receiver, f => receiver.Generate())
                .RuleFor(x => x.Webhook, f => webhook.Generate())
                .RuleFor(x => x.DeliveryAttempts, f => attempt.Generate(2));

            results = new List<MongoWebhookDeliveryResult>();
        }

        private IWebhookDeliveryResultStore<MongoWebhookDeliveryResult> Store
            => Services.GetRequiredService<IWebhookDeliveryResultStore<MongoWebhookDeliveryResult>>();

        public override async Task InitializeAsync() {
            await base.InitializeAsync();

            var fakes = faker.Generate(112).ToList();

            foreach (var attempt in fakes) {
                await CreateAttemptAsync(attempt);

                results.Add(attempt);
            }
        }

        private Task CreateAttemptAsync(MongoWebhookDeliveryResult attempt) {
            return Store.CreateAsync(attempt, default);
        }

        private MongoWebhookDeliveryResult NextRandom()
            => results[Random.Shared.Next(0, results.Count - 1)];

        [Fact]
        public async Task CreateNewResult() {
            var result = faker.Generate();

            var id = await Store.CreateAsync(result, default);

            Assert.NotNull(id);
            Assert.Equal(id, result.Id.ToString());
        }

        [Fact]
        public async Task GetExistingResult() {
            var result = NextRandom();

            var found = await Store.FindByIdAsync(result.Id.ToString(), default);

            Assert.NotNull(found);
            Assert.Equal(result.Id, found.Id);
        }

        [Fact]
        public async Task GetNotExistingResult() {
            var resultId = ObjectId.GenerateNewId();

            var found = await Store.FindByIdAsync(resultId.ToString(), default);

            Assert.Null(found);
        }

        [Fact]
        public async Task DeleteExistingResult() {
            var result = NextRandom();

            var deleted = await Store.DeleteAsync(result, default);

            Assert.True(deleted);

            var found = await Store.FindByIdAsync(result.Id.ToString(), default);

            Assert.Null(found);
        }

        [Fact]
        public async Task DeleteNotExistingResult() {
            var result = faker.Generate();

            await Store.DeleteAsync(result, default);
            
            var found = await Store.FindByIdAsync(result.Id.ToString(), default);
            Assert.Null(found);
        }

        [Fact]
        public async Task CountAll() {
            var count = await Store.CountAllAsync(default);

            Assert.Equal(results.Count, count);
        }

        [Fact]
        public async Task GetByWebhookId() {
            var result = NextRandom();

            var found = await Store.FindByWebhookIdAsync(result.Webhook.WebhookId, default);

            Assert.NotNull(found);
            Assert.Equal(result.Id, found.Id);
        }
    }
}
