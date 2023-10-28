using Bogus;

namespace Deveel.Webhooks {
	class WebhookSubscriptionFaker : Faker<WebhookSubscription> {
		public WebhookSubscriptionFaker(string? tenantId = null) {
			RuleFor(x => x.TenantId, tenantId);
			RuleFor(x => x.SubscriptionId, f => f.Random.Guid().ToString());
			RuleFor(x => x.Name, f => f.Lorem.Word());
			RuleFor(x => x.DestinationUrl, f => f.Internet.Url());
			RuleFor(x => x.EventTypes, f => f.Random.ListItems(EventTypes, 2).ToArray());
			RuleFor(x => x.Status, f => f.Random.Enum<WebhookSubscriptionStatus>());
			RuleFor(x => x.RetryCount, f => f.Random.Int(1, 10));
			RuleFor(x => x.Format, f => f.PickRandom(new[] { "json", "xml" }));
			RuleFor(x => x.Secret, f => f.Internet.Password());
		}

		public static readonly string[] EventTypes = new[] { "data.created", "data.updated", "data.deleted" };
	}
}
