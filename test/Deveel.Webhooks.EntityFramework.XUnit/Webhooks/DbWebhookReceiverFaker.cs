using Bogus;

namespace Deveel.Webhooks {
	public class DbWebhookReceiverFaker : Faker<DbWebhookReceiver> {
		public DbWebhookReceiverFaker() {
			RuleFor(x => x.BodyFormat, f => f.PickRandom(new[] { "json", "xml" }));
			RuleFor(x => x.DestinationUrl, f => f.Internet.UrlWithPath("https"));
			RuleFor(x => x.Headers, f => {
				var headers = new Dictionary<string, string>();
				for (var i = 0; i < f.Random.Int(1, 5); i++) {
					headers[f.Random.Word()] = f.Lorem.Word();
				}

				return headers.Select(x => new DbWebhookReceiverHeader {
					Key = x.Key,
					Value = x.Value
				}).ToList();
			});
		}
	}
}
