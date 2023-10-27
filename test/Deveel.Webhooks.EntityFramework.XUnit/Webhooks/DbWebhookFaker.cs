using System.Text.Json;

using Bogus;

namespace Deveel.Webhooks {
	public class DbWebhookFaker : Faker<DbWebhook> {
		public DbWebhookFaker() {
			RuleFor(x => x.WebhookId, f => f.Random.Guid().ToString());
			RuleFor(x => x.EventType, f => f.PickRandom(EventTypes));
			RuleFor(x => x.TimeStamp, f => f.Date.Past());
			RuleFor(x => x.Data, (f, w) => GenerateData(f, w));
		}

		private string GenerateData(Faker f, DbWebhook webhook) {
			object? data = null;

			switch (webhook.EventType) {
				case "data.created":
					data = new {
						UserId = f.Random.Guid(),
						Email = f.Internet.Email()
					};
					break;
				case "data.deleted":
					data = new {
						UserId = f.Random.Guid()
					};
					break;
				case "data.updated":
					data = new {
						UserId = f.Random.Guid(),
						Email = f.Internet.Email()
					};
					break;
				default:
					break;
			}

			return JsonSerializer.Serialize(data);
		}

		public static readonly string[] EventTypes = new[] { "data.created", "data.deleted", "data.updated" };
	}
}
