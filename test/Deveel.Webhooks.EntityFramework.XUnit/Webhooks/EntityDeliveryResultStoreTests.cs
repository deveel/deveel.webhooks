using System.Runtime.Serialization;
using System.Text.Json;

using Bogus;

using Deveel.Data;

using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

using Xunit.Abstractions;

namespace Deveel.Webhooks {
    public class EntityDeliveryResultStoreTests : EntityWebhookTestBase {
        private readonly Faker<DbWebhookDeliveryResult> faker;
        private readonly List<DbWebhookDeliveryResult> results;

        public EntityDeliveryResultStoreTests(SqliteTestDatabase sqlite, ITestOutputHelper outputHelper) : base(sqlite, outputHelper) {

            var receiver = new Faker<DbWebhookReceiver>()
                .RuleFor(x => x.BodyFormat, f => f.Random.ListItem(new[] { "json", "xml" }))
                .RuleFor(x => x.DestinationUrl, f => f.Internet.Url())
                // .RuleFor(x => x.SubscriptionId, f => f.Random.Guid().OrNull(f)?.ToString())
                .RuleFor(x => x.SubscriptionName, f => f.Name.JobType());

            var webhook = new Faker<DbWebhook>()
                .RuleFor(x => x.EventType, f => f.Random.ListItem(new[] { "data.created", "data.deleted", "data.updated" }))
                .RuleFor(x => x.WebhookId, f => f.Random.Guid().ToString())
                .RuleFor(x => x.TimeStamp, f => f.Date.PastOffset(1))
                .RuleFor(x => x.Data, "{ \"data-type\", \"test\" }");

            var attempt = new Faker<DbWebhookDeliveryAttempt>()
                .RuleFor(x => x.ResponseStatusCode, f => f.Random.ListItem(new int?[] { 200, 201, 204, 400, 404, 500, null }))
                .RuleFor(x => x.StartedAt, (f, a) => f.Date.PastOffset())
                .RuleFor(x => x.EndedAt, (f, a) => a.StartedAt.AddMilliseconds(200).OrNull(f));

            var eventInfo = new Faker<DbEventInfo>()
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

            faker = new Faker<DbWebhookDeliveryResult>()
                .RuleFor(x => x.OperationId, f => f.Random.Guid().ToString())
                .RuleFor(x => x.EventInfo, f => eventInfo.Generate())
                .RuleFor(x => x.Receiver, f => receiver.Generate())
                .RuleFor(x => x.Webhook, f => webhook.Generate())
                .RuleFor(x => x.DeliveryAttempts, f => attempt.Generate(2));

            results = new List<DbWebhookDeliveryResult>();
        }

        private IWebhookDeliveryResultRepository<DbWebhookDeliveryResult> Store
            => Services.GetRequiredService<IWebhookDeliveryResultRepository<DbWebhookDeliveryResult>>();

		public override async Task InitializeAsync() {
            await base.InitializeAsync();

            var fakes = faker.Generate(112).ToList();

            foreach (var attempt in fakes) {
                await AddAttemptAsync(attempt);

                results.Add(attempt);
            }
        }

        private Task AddAttemptAsync(DbWebhookDeliveryResult attempt) {
            return Store.AddAsync(attempt, default);
        }

        private DbWebhookDeliveryResult NextRandom()
            => results[Random.Shared.Next(0, results.Count - 1)];

        [Fact]
        public async Task CreateNewResult() {
            var result = faker.Generate();

            await Store.AddAsync(result);

			Assert.NotNull(result.Id);
        }

        [Fact]
        public async Task GetExistingResult() {
            var result = NextRandom();

            var found = await Store.FindByKeyAsync(result.Id!);

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

            var found = await Store.FindByKeyAsync(resultId!);

            Assert.Null(found);
        }

        [Fact]
        public async Task RemoveExistingResult() {
            var result = NextRandom();

            var deleted = await Store.RemoveAsync(result);

            Assert.True(deleted);

            var found = await Store.FindByKeyAsync(result.Id!);

            Assert.Null(found);
        }

        [Fact]
        public async Task RemoveNotExistingResult() {
            var resultId = Random.Shared.Next(results.Max(x => x.Id!.Value) + 1, Int32.MaxValue);
            var result = faker.Generate();
            result.Id = resultId;

            var removed = await Store.RemoveAsync(result);

			Assert.False(removed);
        }

        [Fact]
        public async Task CountAll() {
            var count = await Store.CountAllAsync();

            Assert.Equal(results.Count, count);
        }

        [Fact]
        public async Task GetByWebhookId() {
            var result = NextRandom();

            var found = await Store.FindByWebhookIdAsync(result.Webhook.WebhookId!, default);

            Assert.NotNull(found);
            Assert.Equal(result.Id, found.Id);
        }
    }
}
