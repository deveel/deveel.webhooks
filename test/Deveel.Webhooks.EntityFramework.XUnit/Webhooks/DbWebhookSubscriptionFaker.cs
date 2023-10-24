using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
		}

		public static readonly string[] EventTypes = new[] { "data.created", "data.deleted", "data.updated" };
	}
}
