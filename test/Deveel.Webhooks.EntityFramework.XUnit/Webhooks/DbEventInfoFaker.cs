using System.Text.Json;

using Bogus;

namespace Deveel.Webhooks {
	public class DbEventInfoFaker : Faker<DbEventInfo> {
		public DbEventInfoFaker() {
			RuleFor(x => x.EventId, f => f.Random.Guid().ToString());
			RuleFor(x => x.EventType, f => f.Random.ListItem(new[] { "created", "deleted", "updated" }));
			RuleFor(x => x.DataVersion, "1.0");
			RuleFor(x => x.EventId, f => f.Random.Guid().ToString("N"));
			RuleFor(x => x.TimeStamp, f => f.Date.PastOffset());
			RuleFor(x => x.Subject, "data");
			RuleFor(x => x.Data, f => JsonSerializer.Serialize(new {
					data_type = f.Random.Word(),
					users = f.Random.ListItems(new string[] { "user1", "user2" })
				}));
		}
	}
}
