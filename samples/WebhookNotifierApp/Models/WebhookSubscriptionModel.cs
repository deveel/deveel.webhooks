using MongoFramework.Attributes;

namespace Deveel.Webhooks.Models {
	public class WebhookSubscriptionModel {
		public string Id { get; set; }

		public string Url { get; set; }

		public string Secret { get; set; }

		public string[] Events { get; set; }

		public IDictionary<string, string> Headers { get; set; }

		public DateTimeOffset? Created { get; set; }

		public DateTimeOffset? Updated { get; set; }
	}
}
