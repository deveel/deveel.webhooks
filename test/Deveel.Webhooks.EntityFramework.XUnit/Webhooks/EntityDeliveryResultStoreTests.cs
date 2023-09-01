using System.Text.Json;

using Bogus;

using Microsoft.Extensions.DependencyInjection;

using Xunit.Abstractions;

namespace Deveel.Webhooks {
    public class EntityDeliveryResultStoreTests : EntityWebhookTestBase {
        private readonly Faker<WebhookDeliveryResultEntity> faker;
        private readonly List<WebhookDeliveryResultEntity> results;

        public EntityDeliveryResultStoreTests(SqliteTestDatabase sqlite, ITestOutputHelper outputHelper) : base(sqlite, outputHelper) {

            var receiver = new Faker<WebhookReceiverEntity>()
                .RuleFor(x => x.BodyFormat, f => f.Random.ListItem(new[] { "json", "xml" }))
                .RuleFor(x => x.DestinationUrl, f => f.Internet.Url())
                // .RuleFor(x => x.SubscriptionId, f => f.Random.Guid().OrNull(f)?.ToString())
                .RuleFor(x => x.SubscriptionName, f => f.Name.JobType());

            var webhook = new Faker<WebhookEntity>()
                .RuleFor(x => x.EventType, f => f.Random.ListItem(new[] { "data.created", "data.deleted", "data.updated" }))
                .RuleFor(x => x.WebhookId, f => f.Random.Guid().ToString())
                .RuleFor(x => x.TimeStamp, f => f.Date.PastOffset(1))
                .RuleFor(x => x.Data, "{ \"data-type\", \"test\" }");

            var attempt = new Faker<WebhookDeliveryAttemptEntity>()
                .RuleFor(x => x.ResponseStatusCode, f => f.Random.ListItem(new int?[] { 200, 201, 204, 400, 404, 500, null }))
                .RuleFor(x => x.StartedAt, (f, a) => f.Date.PastOffset())
                .RuleFor(x => x.EndedAt, (f, a) => a.StartedAt.AddMilliseconds(200).OrNull(f));

            var eventInfo = new Faker<EventInfoEntity>()
                .RuleFor(x => x.EventId, f => f.Random.Guid().ToString())
                .RuleFor(x => x.EventType, f => f.Random.ListItem(new[] { "created", "deleted", "updated" }))
                .RuleFor(x => x.DataVersion, "1.0")
                .RuleFor(x => x.EventId, f => f.Random.Guid().ToString("N"))
                .RuleFor(x => x.TimeStamp, f => f.Date.PastOffset())
                .RuleFor(x => x.Subject, "data")
                .RuleFor(x => x.Data, f => JsonSerializer.Serialize(new {
                    data_type = f.Random.Word(),
                    users = f.Random.ListItems(new string[]{ "user1", "user2" })
                }));

            faker = new Faker<WebhookDeliveryResultEntity>()
                .RuleFor(x => x.OperationId, f => f.Random.Guid().ToString())
                .RuleFor(x => x.EventInfo, f => eventInfo.Generate())
                .RuleFor(x => x.Receiver, f => receiver.Generate())
                .RuleFor(x => x.Webhook, f => webhook.Generate())
                .RuleFor(x => x.DeliveryAttempts, f => attempt.Generate(2))
                .FinishWith((f, x) => x.EventId = x.EventInfo.EventId);

            results = new List<WebhookDeliveryResultEntity>();
        }

        private IWebhookDeliveryResultStore<WebhookDeliveryResultEntity> Store
            => Services.GetRequiredService<IWebhookDeliveryResultStore<WebhookDeliveryResultEntity>>();

        public override async Task InitializeAsync() {
            await base.InitializeAsync();

            var fakes = faker.Generate(112).ToList();

            foreach (var attempt in fakes) {
                await CreateAttemptAsync(attempt);

                results.Add(attempt);
            }
        }

        private Task CreateAttemptAsync(WebhookDeliveryResultEntity attempt) {
            return Store.CreateAsync(attempt, default);
        }

        private WebhookDeliveryResultEntity NextRandom()
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
            var resultId = Random.Shared.Next(results.Max(x => x.Id!.Value) + 1, Int32.MaxValue);

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
            var resultId = Random.Shared.Next(results.Max(x => x.Id!.Value) + 1, Int32.MaxValue);
            var result = faker.Generate();
            result.Id = resultId;

            await Assert.ThrowsAsync<WebhookEntityException>(() => Store.DeleteAsync(result, default));
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
