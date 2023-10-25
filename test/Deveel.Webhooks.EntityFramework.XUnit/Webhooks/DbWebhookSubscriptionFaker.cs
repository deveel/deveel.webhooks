using Bogus;

namespace Deveel.Webhooks {
	public class DbWebhookSubscriptionFaker : Faker<DbWebhookSubscription> {
		public DbWebhookSubscriptionFaker() {
			RuleFor(x => x.Id, f => f.Random.Guid().ToString());
			RuleFor(x => x.Name, f => f.Lorem.Word());
			RuleFor(x => x.DestinationUrl, f => f.Internet.Url());
			RuleFor(x => x.Status, f => f.Random.Enum<WebhookSubscriptionStatus>());
			RuleFor(x => x.Format, f => f.PickRandom<string>("json", "xml"));
			RuleFor(x => x.Filters, f => new List<DbWebhookFilter> {
				new DbWebhookFilter {
					Format = "linq",
					Expression = WebhookFilter.Wildcard
				}
			});
			RuleFor(x => x.Events, f => {
				var f2 = new Faker<DbWebhookSubscriptionEvent>()
					.RuleFor(x => x.EventType, f => f.PickRandom(EventTypes));

				return f2.Generate(2);
			});
			RuleFor(x => x.Headers, f => {
				var headers = new Dictionary<string, string>();
				for (var i = 0; i < f.Random.Int(1, 5); i++) {
					headers[f.Random.Word()] = f.Lorem.Word();
				}

				return headers.Select(x => new DbWebhookSubscriptionHeader {
					Key = x.Key,
					Value = x.Value
				}).ToList();
			});
			RuleFor(x => x.Properties, f => {
				var properties = new Dictionary<string, object?>();
				for (var i = 0; i < f.Random.Int(1, 5); i++) {
					properties[f.Random.Word()] = f.Random.Bool() ? f.Random.Word() : f.Random.Int();
				}

				return properties.Select(x => new DbWebhookSubscriptionProperty {
					Key = x.Key,
					Value = DbWebhookValueConvert.ConvertToString(x.Value),
					ValueType = DbWebhookValueConvert.GetValueType(x.Value)
				}).ToList();
			});
		}

		public static readonly string[] EventTypes = new[] { "data.created", "data.deleted", "data.updated" };
	}
}
