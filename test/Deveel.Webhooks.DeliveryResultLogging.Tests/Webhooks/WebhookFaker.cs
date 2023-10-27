using Bogus;

namespace Deveel.Webhooks {
	class WebhookFaker : Faker<Webhook> {
		public WebhookFaker() {
			RuleFor(x => x.Id, f => f.Random.Guid().ToString());
			RuleFor(x => x.Name, f => f.Lorem.Word());
			RuleFor(x => x.EventType, f => f.PickRandom(EventTypes));
			RuleFor(x => x.SubscriptionId, f => f.Random.Guid().ToString());
			RuleFor(x => x.TimeStamp, f => f.Date.Past());
		}

		public static readonly string[] EventTypes = new[] { "data.created", "data.modified" };
	}
}
