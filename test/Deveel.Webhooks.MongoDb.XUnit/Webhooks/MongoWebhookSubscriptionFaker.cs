using Bogus;

namespace Deveel.Webhooks {
	public class MongoWebhookSubscriptionFaker : Faker<MongoWebhookSubscription> {
		public MongoWebhookSubscriptionFaker(string? tenantId = null) {
			RuleFor(x => x.TenantId, tenantId);
			RuleFor(x => x.Name, f => f.Name.JobTitle());
			RuleFor(x => x.EventTypes, f => f.Random.ListItems(EventTypes));
			RuleFor(x => x.Format, f => f.Random.ListItem(new[] { "json", "xml" }));
			RuleFor(x => x.DestinationUrl, f => f.Internet.UrlWithPath("https"));
			RuleFor(x => x.Status, f => f.Random.Enum<WebhookSubscriptionStatus>());
			RuleFor(x => x.Filters, f => new List<MongoWebhookFilter> {
				new MongoWebhookFilter{ Format = "linq", Expression = WebhookFilter.Wildcard }
			});
			RuleFor(x => x.Headers, f => {
				var headers = new Dictionary<string, string>();
				for (var i = 0; i < f.Random.Int(1, 5); i++) {
					headers[f.Random.Word()] = f.Lorem.Word();
				}

				return headers;
			});
		}

		public static readonly string[] EventTypes = new[] { "data.created", "data.deleted", "data.updated" };
	}
}
