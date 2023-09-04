using Bogus;

using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

using Xunit.Abstractions;

namespace Deveel.Webhooks {
    public class WebhookSubscriptionManagementTests : EntityWebhookTestBase {
        private IList<DbWebhookSubscription> subscriptions;
        private Faker<DbWebhookSubscription> faker;

        public WebhookSubscriptionManagementTests(SqliteTestDatabase sqlite, ITestOutputHelper outputHelper) 
            : base(sqlite, outputHelper) {
            //var subsEvents = new Faker<WebhookEventSubscription>()
            //    .RuleFor(x => x.EventType, f => f.Random.ListItem(new[] { "data.created", "data.deleted", "data.updated" }));

            faker = new Faker<DbWebhookSubscription>()
                .RuleFor(x => x.Name, f => f.Name.JobTitle())
                .RuleFor(x => x.Events, f => {
                    var f2 = new Faker<DbWebhookSubscriptionEvent>()
                        .RuleFor(x => x.EventType, f => f.Random.ListItem(new[] { "data.created", "data.deleted", "data.updated" }));

                    return f2.Generate(2);
                })
                .RuleFor(x => x.Format, f => f.Random.ListItem(new[] { "json", "xml" }))
                .RuleFor(x => x.DestinationUrl, f => f.Internet.UrlWithPath("https"))
                .RuleFor(x => x.Status, f => f.Random.Enum<WebhookSubscriptionStatus>())
                .RuleFor(x => x.Filters, f => new List<DbWebhookFilter> {
                    new DbWebhookFilter{ Format = "linq", Expression = WebhookFilter.Wildcard }
                });

            subscriptions = new List<DbWebhookSubscription>();
        }

        private WebhookSubscriptionManager<DbWebhookSubscription> Manager
            => Services.GetRequiredService<WebhookSubscriptionManager<DbWebhookSubscription>>();

        protected IWebhookSubscriptionStore<DbWebhookSubscription> Store
            => Services.GetRequiredService<IWebhookSubscriptionStore<DbWebhookSubscription>>();

        public override async Task InitializeAsync() {
            await base.InitializeAsync();

            var fakes = faker.Generate(112).ToList();

            foreach (var subscription in fakes) {
                await Store.CreateAsync(subscription, default);

                subscriptions.Add(subscription);
            }
        }

        private DbWebhookSubscription RandomSubscription()
            => subscriptions[Random.Shared.Next(0, subscriptions.Count - 1)];

        [Fact]
        public async Task AddSubscription() {
            var subscription = faker.Generate();

            var subscriptionId = await Manager.CreateAsync(subscription);

            Assert.NotNull(subscriptionId);
            Assert.Null(subscription.TenantId);
            Assert.NotEmpty(subscription.Events);
            Assert.NotEmpty(subscription.Filters);
        }

        [Theory]
        [InlineData("")]
        [InlineData(null)]
        [InlineData("ftp://dest.example.com")]
        [InlineData("test data")]
        public async Task AddSubscriptionWithInvalidDestination(string url) {
            var subscription = faker.Generate();
            subscription.DestinationUrl = url;

            var error = await Assert.ThrowsAsync<WebhookSubscriptionValidationException>(
                () => Manager.CreateAsync(subscription));

            Assert.NotNull(error);
            Assert.NotNull(error.Errors);
            Assert.NotEmpty(error.Errors);
            Assert.Single(error.Errors);
        }

        [Fact]
        public async Task RemoveExistingSubscription() {
            var subscription = RandomSubscription();

            var result = await Manager.DeleteAsync(subscription);

            Assert.True(result);
        }

        [Fact]
        public async Task GetExistingSubscription() {
            var subscriptionId = RandomSubscription().Id.ToString();

            var subscription = await Manager.FindByIdAsync(subscriptionId);

            Assert.NotNull(subscription);
            Assert.Equal(subscriptionId, subscription.Id.ToString());
            Assert.NotEmpty(subscription.Events);
            Assert.NotNull(subscription.Filters);
            Assert.NotEmpty(subscription.Filters);
            Assert.Single(subscription.Filters);
            Assert.True(subscription.Filters.First().IsWildcard());
        }

        [Fact]
        public async Task GetNotExistingSubscription() {
            var subscriptionId = Guid.NewGuid().ToString();

            var subscription = await Manager.FindByIdAsync(subscriptionId);

            Assert.Null(subscription);
        }

        [Fact]
        public async Task GetPageOfSubscriptions() {
            var totalPages = (int)Math.Ceiling(subscriptions.Count / (double)10);

            Assert.True(Manager.SupportsPaging);

            var query = new PagedQuery<DbWebhookSubscription>(1, 10);
            var result = await Manager.GetPageAsync(query);

            Assert.NotNull(result);
            Assert.NotEmpty(result.Items);
            Assert.Equal(subscriptions.Count, result.TotalCount);
            Assert.Equal(totalPages, result.TotalPages);
            Assert.Equal(subscriptions[0].Id.ToString(), result.Items[0].Id.ToString());
            Assert.Equal(subscriptions[1].Id.ToString(), result.Items[1].Id.ToString());
        }

        [Fact]
        public async Task QuerySubscriptionsByEventType() {
            var dataCreatedSubs = subscriptions.Where(x => x.Events.Any(y => y.EventType == "data.created")).ToList();
            var result = await Manager.Subscriptions.Where(x => x.Events.Any(y => y.EventType == "data.created")).ToListAsync();

            Assert.Equal(dataCreatedSubs.Count, result.Count);
        }

        [Fact]
        public async Task GetSubscriptionsByEventType() {
            var dataCreatedSubs = subscriptions.Where(x => x.Events.Any(y => y.EventType == "data.created") &&  x.Status == WebhookSubscriptionStatus.Active).ToList();
            var result = await Store.GetByEventTypeAsync("data.created", true, default);

            Assert.Equal(dataCreatedSubs.Count, result.Count);
        }


        [Fact]
        public async Task ActivateExistingSubscription() {
            var subscription = subscriptions.First(x => x.Status == WebhookSubscriptionStatus.Suspended);

            var result = await Manager.EnableAsync(subscription);

            Assert.True(result);

            Assert.Equal(WebhookSubscriptionStatus.Active, subscription.Status);
        }

        [Fact]
        public async Task ActivateAlreadyActiveSubscription() {
            var subscription = subscriptions.First(x => x.Status == WebhookSubscriptionStatus.Active);

            var result = await Manager.EnableAsync(subscription);

            Assert.False(result);
            Assert.Equal(WebhookSubscriptionStatus.Active, subscription.Status);
        }

        [Fact]
        public async Task DisableExistingSubscription() {
            var subscription = subscriptions.First(x => x.Status == WebhookSubscriptionStatus.Active);

            var result = await Manager.DisableAsync(subscription);

            Assert.True(result);
            Assert.Equal(WebhookSubscriptionStatus.Suspended, subscription.Status);

            await Manager.UpdateAsync(subscription);
        }

        [Fact]
        public async Task DisableAlreadyDisabledSubscription() {
            var subscription = subscriptions.First(x => x.Status == WebhookSubscriptionStatus.Suspended);

            var result = await Manager.DisableAsync(subscription);

            Assert.False(result);
            Assert.Equal(WebhookSubscriptionStatus.Suspended, subscription.Status);
        }

        [Fact]
        public async Task CountAllSubscriptionExistingTenant() {
            var count = subscriptions.Count;

            var result = await Manager.CountAllAsync();

            Assert.Equal(count, result);
        }

        [Fact]
        public async Task QueryAllNonActive() {
            var count = subscriptions.Count(x => x.Status != WebhookSubscriptionStatus.Active);

            Assert.True(Manager.SupportsQueries);
            var result = await Manager.Subscriptions.Where(x => x.Status != WebhookSubscriptionStatus.Active).ToListAsync();

            Assert.Equal(count, result.Count);
            Assert.True(result.All(x => x.Status != WebhookSubscriptionStatus.Active));
        }
    }
}
